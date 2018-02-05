using UnityEngine;
using UnityEditor;
using MonoRAILProxy;
using Microsoft.FSharp.Core;
using System;
using System.IO;

public class CADImporterWizard : ScriptableWizard { 

    const string m_sRequiredAssembyVersion = "0.6.0.0"; // バージョンチェック用。MonoRAILProxy, MoNoImporter の FileVersionアセンブリ情報文字列と合わせること。

    private string importAssetPath;
    private string statusText;
    static private CADManager cadManager = new CADManager();
    static private LicenseWizard licenseWizard = null;
    // Tessellation Tolerance
    readonly TessParamGUI m_tessParam = new TessParamGUI();

    string[] filePaths;

    bool firstPass = true;

    int counter = 0;

    int numberOfFiles;

    CADImporterWizard() {

    }

    [MenuItem("Assets/CAD Importer")]
    static void OpenWizard() {

        DisplayWizard<CADImporterWizard>("CAD Importer", "Done", "Import");

        // ライセンス認証（ライセンスファイルの有無とライセンスの一致を確認する）
        if (cadManager.CheckLicense() == false) {

            Debug.Log("License check NG.");
            licenseWizard = DisplayWizard<LicenseWizard>("License Installer", "Cancel", "Install");
            licenseWizard.CadManager = cadManager;
            licenseWizard.ShowPopup();
        }
        else {

            Debug.Log("License check OK.");

            // Version check
            var sVer = cadManager.CheckVersion();
            if (sVer != m_sRequiredAssembyVersion) {

                Debug.Log("Version Check Failed: " + sVer);
                EditorUtility.DisplayDialog("Error", "Version mismatch. Please extract Mono.RAIL.zip on root folder.", "OK");
            }
            else
                Debug.Log("Version Check OK: " + sVer);
        }
    }

    // Import Button
    void OnWizardOtherButton() {

        bool directoryExists = Directory.Exists(importAssetPath);

        if(directoryExists && firstPass) {

            filePaths = Directory.GetFiles(importAssetPath);

            numberOfFiles = filePaths.Length;

            firstPass = false;
        }

        Debug.Log(counter);
        bool fileExists = File.Exists(filePaths[counter]);

        if(fileExists) {

            statusText = "Import processing...";

            if (cadManager.Import(filePaths[counter], m_tessParam)) {

                var startTime = DateTime.Now;

                // エディタ上で進捗更新をチェック
                EditorApplication.CallbackFunction update = null;
                update = () => {

                    if (cadManager.isDone() == false) {

                        // 毎フレームチェック
                        int progress = cadManager.GetProgress();
                        var sPercent = string.Format("{0:0}%", progress);
                        //Debug.Log("Progress: " + sPercent);
                        if (EditorUtility.DisplayCancelableProgressBar("Importing progress", "import processing..." + sPercent, progress / 100.0f) != false) {

                            // インポート処理中断
                            cadManager.Close();
                            EditorUtility.ClearProgressBar();
                            EditorApplication.update -= update;
                            if (cadManager.GetExitCode() != 0) {
                                statusText = "Canceled";
                                Debug.Log(statusText);
                            }
                        }
                    }
                    else {
                    // インポート処理終了
                        cadManager.Close();
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update -= update;
                        if (cadManager.GetExitCode() != 0) {

                            statusText = "Failed";
                            Debug.Log(statusText);
                            // cache ディレクトリにある "Debug.txt"を開いて先頭行にある例外名を取得し、そのメッセージを表示する
                            var sErrorMessage = cadManager.GetErrorMessage();
                            if (!string.IsNullOrEmpty(sErrorMessage)) {

                                Debug.LogError(sErrorMessage);

                                if (sErrorMessage == "System.OutOfMemoryException")
                                    EditorUtility.DisplayDialog("Error", "Memory Overflow", "OK");                            
                            }
                    }
                    else {
                        // メッシュデータを復元する
                        int nVertices, nFacets;
                        if (cadManager.GetMesh(out nVertices, out nFacets) == false) {

                            statusText = "Failed";
                            Debug.LogWarning(statusText);
                            Debug.LogWarning("Can't import file:" + filePaths[counter]);
                        }
                        else {

                            statusText = "Succeeded";
                            Debug.Log(statusText);

                            var duration = DateTime.Now - startTime;
                            Debug.Log("Duration: " + duration.ToString());
                            Debug.Log(string.Format("Vertices: {0}, Facets: {1}", nVertices, nFacets));

                            // Keep iterating until all files are imported
                            if(counter < numberOfFiles) {

                                OnWizardOtherButton();

                                counter++;
                            }
                        }
                    }
                    }
                };
                EditorApplication.update += update;
            }
            else {
                statusText = "Failed";
                Debug.LogWarning("Can't import file:" + filePaths[counter]);
            }
        }
        else {
            statusText = "Failed";
            Debug.LogWarning("Can't read file:" + filePaths[counter]);
        }
    }

    // Done Button
    void OnWizardCreate() {

        if (licenseWizard != null)
            licenseWizard.Close();

        cadManager.Close();
        Debug.Log("Close");
    }

    protected override bool DrawWizardGUI() {

        EditorGUILayout.LabelField("CAD Import Settings");
        importAssetPath = EditorGUILayout.TextField("Import Asset Path", importAssetPath);

        // Tessellation tolerance
        //m_tessParam.DrawGUI();

        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField( "Status ", statusText );
        //EditorGUILayout.Space();
    
        /*var evt = Event.current;
        var dropArea = GUILayoutUtility.GetRect(
                        GUIContent.none,
                        GUIStyle.none,
                        GUILayout.ExpandHeight(true),
                        GUILayout.MinHeight(100));
        GUI.Box(dropArea, "Drag & Drop");

        int id = GUIUtility.GetControlID(FocusType.Passive);
        switch (evt.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
            if (!dropArea.Contains(evt.mousePosition))
                break;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            DragAndDrop.activeControlID = id;

            if (evt.type == EventType.DragPerform) {

                DragAndDrop.AcceptDrag();
                Debug.Log("Drop!");
                foreach (string path in DragAndDrop.paths) {
                // ToDo: 複数個対
                if (Path.IsPathRooted(path))
                    importAssetPath = path;
                else
                    importAssetPath = Application.dataPath + "/../" + path;
                Debug.Log("Droped:" + path);
                }
                DragAndDrop.activeControlID = 0;
            }
            Event.current.Use();
            break;
        }*/
        return base.DrawWizardGUI();
    }
}
