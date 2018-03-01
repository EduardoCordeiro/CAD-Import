using UnityEngine;
using UnityEditor;
using MonoRAILProxy;
using Microsoft.FSharp.Core;
using System;
using System.IO;

using CAD.Support;

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

    GameObject rootObject;

    HierarchyCreator hierarchyCreator;

    CADImporterWizard() {

    }

    private void OnEnable() {

        rootObject = new GameObject();
        rootObject.AddComponent<HierarchyCreator>();

        hierarchyCreator = rootObject.GetComponent<HierarchyCreator>();
    }

    [MenuItem("Assets/CAD Importer")]
    static void OpenWizard() {

        DisplayWizard<CADImporterWizard>("CAD Importer", "Exit", "Import");

        if (cadManager.CheckLicense() == false) {

            Debug.Log("License check NG.");

            licenseWizard = DisplayWizard<LicenseWizard>("License Installer", "Cancel", "Install");
            licenseWizard.CadManager = cadManager;
            licenseWizard.ShowPopup();
        }
        else {

            Debug.Log("License check OK.");

            // Version check
            string sVer = cadManager.CheckVersion();

            if (sVer != m_sRequiredAssembyVersion) {

                Debug.Log("Version Check Failed: " + sVer);
                EditorUtility.DisplayDialog("Error", "Version mismatch. Please extract Mono.RAIL.zip on root folder.", "OK");
            }
            else
                Debug.Log("Version Check OK: " + sVer);
        }
    }

    void FirstPass() {

        filePaths = Directory.GetFiles(importAssetPath);

        numberOfFiles = filePaths.Length;

        // Update rootObject name
        string[] split = importAssetPath.Split('/');
        // folder structure matters here
        rootObject.name = split[split.Length - 3];

        firstPass = false;
    }

    // Import Button
    void OnWizardOtherButton() {

        bool directoryExists = Directory.Exists(importAssetPath);

        if(directoryExists && firstPass)
            FirstPass();

        string filePath = filePaths[counter];

        bool fileExists = File.Exists(filePath);

        if(fileExists) {

            statusText = "Import processing...";

            if (cadManager.Import(filePath, m_tessParam)) {

                var startTime = DateTime.Now;

                EditorApplication.CallbackFunction update = null;

                update = () => {

                    if (cadManager.isDone() == false) {
                        
                        int progress = cadManager.GetProgress();
                        var sPercent = string.Format("{0:0}%", progress);

                        if (EditorUtility.DisplayCancelableProgressBar("Importing progress", "import processing..." + sPercent, progress / 100.0f) != false) {

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
                        
                        cadManager.Close();

                        EditorUtility.ClearProgressBar();

                        EditorApplication.update -= update;

                        if (cadManager.GetExitCode() != 0) {

                            statusText = "Failed";
                            Debug.Log(statusText);

                            string sErrorMessage = cadManager.GetErrorMessage();

                            if (!string.IsNullOrEmpty(sErrorMessage)) {

                                Debug.LogError(sErrorMessage);

                                if (sErrorMessage == "System.OutOfMemoryException")
                                    EditorUtility.DisplayDialog("Error", "Memory Overflow", "OK");                            
                            }
                    }
                    else {
                            
                        int nVertices, nFacets;

                        if (cadManager.GetMesh(out nVertices, out nFacets) == false) {

                            statusText = "Failed";
                            Debug.LogWarning(statusText);
                            Debug.LogWarning("Can't import file:" + filePath);
                        }
                        else {

                            statusText = "File : " + filePath + "was imported sucessfully ";

                            TimeSpan duration = DateTime.Now - startTime;
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
                Debug.LogWarning("Can't import file:" + filePath);
            }
        }
        else {

            statusText = "Failed";
            Debug.LogWarning("Can't read file:" + filePath);
        }
    }

    // Done Button
    void OnWizardCreate() {
        
        hierarchyCreator.CreateHierarchy();

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
