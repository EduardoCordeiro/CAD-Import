using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using MonoRAILProxy;
using UnityEditor;

namespace Assets
{
    public class MeshImporter : ScriptableWizard
    {
        private string importAssetPath;

        static private CADManager cadManager = new CADManager();

        private OBJImporter importer;
        private string[] filePaths;
        private int selectionGridInteger;
        private MeshFilter meshFilter;
        private GameObject gameObject;

        MeshImporter()
        {
        }



        [MenuItem("Assets/Mesh Importer by Katia")]
        static void OpenWizard()
        {
            DisplayWizard<MeshImporter>("Mesh Importer", "Exit", "Import");
            
        }

        void OnWizardOtherButton()
        {
            importer = new OBJImporter();
            filePaths = Directory.GetFiles(importAssetPath);

            foreach (var file in filePaths)
            {
                Debug.Log(file);

                var holderMesh = new Mesh();
                var newMesh = new OBJImporter();
                holderMesh = newMesh.ImportFile(file);
                Debug.Log("Mesh importata");


                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                filter.mesh = holderMesh;

                Debug.Log("Mesh creata");

            }


        }

   
        protected override bool DrawWizardGUI()
        {

            EditorGUILayout.LabelField("CAD Import Settings");
            importAssetPath = EditorGUILayout.TextField("Import Asset Path", importAssetPath);

            return base.DrawWizardGUI();
        }
    }
}