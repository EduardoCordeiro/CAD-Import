using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using CAD.Support;
using MonoRAILProxy;
using UnityEditor;
using Collider = Assets.Scripts.Support.Collider;

namespace Assets
{
    public class MeshImporter : ScriptableWizard
    {
        private string importAssetPath;

        static private CADManager cadManager = new CADManager();

        private OBJImporter importer;
        private string[] meshesPath;
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
            //importer = new OBJImporter();
            foreach (var modelDirectory in Directory.GetDirectories(importAssetPath))
            {
                var assemblyName = modelDirectory.Split('\\').Last();
                var newAssembly = new GameObject(assemblyName);
                newAssembly = GameObject.Find(assemblyName);
                
                meshesPath = Directory.GetFiles(modelDirectory + "\\STLforX3D");

                foreach (var mesh in meshesPath)
                {

                    var meshName = Path.GetFileNameWithoutExtension(mesh);
                    var resourceToLoad = string.Format("AssemblyDataset\\{0}\\{1}", assemblyName, meshName);

                    //var holderMesh = new Mesh();
                    //var newMesh = new OBJImporter();
                    //holderMesh = newMesh.ImportFile(file);
                    var holderMesh = Resources.Load<Mesh>(resourceToLoad);

                    if (holderMesh == null)
                        Debug.Log("Mesh nulla per " + meshName);

                    var newPart = new GameObject(meshName);

                    GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    primitive.active = false;
                    Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
                    DestroyImmediate(primitive);

                    MeshRenderer renderer = newPart.AddComponent<MeshRenderer>();
                    renderer.sharedMaterial = diffuse;

                    MeshFilter filter = newPart.AddComponent<MeshFilter>();
                    filter.mesh = holderMesh;
                    newPart.transform.parent = newAssembly.transform;

                }
                HierarchyCreator newAssemblyHierarchy = newAssembly.AddComponent<HierarchyCreator>();
                newAssemblyHierarchy.CreateHierarchy();

                CenteringInBoundingBox(newAssembly);


            }
        }

        private static void CenteringInBoundingBox(GameObject newAssembly)
        {
            Vector3 maxPoint;
            Vector3 minPoint;
            Vector3 centerAveragePoint;
            Collider.ComputeBoundingBox(newAssembly, out maxPoint, out minPoint, out centerAveragePoint);
            Debug.Log("Centro " + -centerAveragePoint.x + " " + -centerAveragePoint.y + " " + -centerAveragePoint.z);
            Vector3 m2mm = new Vector3((float) 0.001, (float) 0.001, (float) 0.001);
            //newAssembly.transform.position.Scale(m2mm);
            newAssembly.transform.localScale = m2mm;
            newAssembly.transform.position = new Vector3(-centerAveragePoint.x * m2mm.x, -centerAveragePoint.y * m2mm.y,
                -centerAveragePoint.z * m2mm.z);
            Debug.Log("Modello aggiornato");
        }


        protected override bool DrawWizardGUI()
        {

            EditorGUILayout.LabelField("CAD Import Settings");
            importAssetPath = EditorGUILayout.TextField("Import Asset Path", importAssetPath);

            return base.DrawWizardGUI();
        }
    }
}