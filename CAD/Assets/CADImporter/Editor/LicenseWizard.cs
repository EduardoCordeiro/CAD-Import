using UnityEngine;
using UnityEditor;
using MonoRAILProxy;

public class LicenseWizard : ScriptableWizard
{
  public string LicenseFilePath
  {
    get; set;
  }
  public CADManager CadManager
  {
    get; set;
  }

  void OnWizardOtherButton()
  {
    bool isExist = System.IO.File.Exists(LicenseFilePath);
    if (isExist) {
      Debug.Log("Install");
      if (CadManager.InstallLicense(LicenseFilePath) != false) {
        Debug.Log("License installed.");
        Close();
      }
      else {
        Debug.Log("License install failed.");
      }
    }
  }

  void OnWizardCreate()
  {
    Debug.Log("Close");
  }

  protected override bool DrawWizardGUI()
  {
    LicenseFilePath = EditorGUILayout.TextField("License File Path", LicenseFilePath);

    var evt = Event.current;
    var dropArea = GUILayoutUtility.GetRect(
        GUIContent.none,
        GUIStyle.none,
        GUILayout.ExpandHeight(true),
        GUILayout.MinHeight(50));

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
            LicenseFilePath = path;
            Debug.Log("Droped:" + path);
          }
          DragAndDrop.activeControlID = 0;
        }
        Event.current.Use();
        break;
    }
    return base.DrawWizardGUI();
  }
}
