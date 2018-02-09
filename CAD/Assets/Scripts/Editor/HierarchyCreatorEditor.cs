using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using CAD.Utility;

[CustomEditor(typeof(HierarchyCreator))]
public class HierarchyCreatorEditor : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        HierarchyCreator hierarchyCreator = (HierarchyCreator)target;

        if(GUILayout.Button("Create Hierarchy!"))
            hierarchyCreator.CreateHierarchy();
    }
}
