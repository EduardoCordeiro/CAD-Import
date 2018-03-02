using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;

namespace CAD.Support {

    public class HierarchyCreator : MonoBehaviour {

        // Use this for initialization
        void Start() {

            CalculateBoudingBox();
        }

        // Update is called once per frame
        void Update() {

        }

        private void CalculateBoudingBox() {

            //foreach(Transform child in transform)
                //CreateBoxCollider(child);            
        }

        private void CreateBoxCollider(Transform meshTransform) {

            MeshFilter meshFilter = meshTransform.GetComponent<MeshFilter>();

            if(meshFilter == null) {

                //Find new childs
                foreach(Transform child in meshTransform)
                    CreateBoxCollider(child);
            }
            else {

                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

                BoxCollider box = meshTransform.gameObject.AddComponent<BoxCollider>();
                bounds.Encapsulate(meshFilter.mesh.bounds);
                bounds.center = Vector3.zero;

                //box.size = bounds.size;
                //box.center = bounds.center;
                box.size = meshFilter.mesh.bounds.max - meshFilter.mesh.bounds.min;
                // center NOT WORKING
                //box.center = meshTransform.transform.position;
            }

        }

        public void CreateHierarchy() {

            Debug.Log("CreateHierarchy()");

            // Virtual Children are the best
            MakeChild();

            // Name is clear
            ClearDuplicateGameObjects();

            // idem idem
            Hierarchy();
        }

        /// <summary>
        /// Create the object hierarchy
        /// </summary>
        private void Hierarchy() {

            List<Transform> children = new List<Transform>();

            foreach(Transform child in this.transform)
                children.Add(child);

            foreach(Transform c in children) {

                List<string> name = c.name.Split(' ').ToList();

                GameObject currentObject = c.gameObject;

                c.name = name.Last();

                name.Remove(name.Last());

                while(name.Count > 0) { 

                    GameObject parent = GameObject.Find(name.Last());
                    
                    if(parent == null) {

                        parent = new GameObject(name.Last());
                        parent.transform.parent = this.transform;
                    }

                    currentObject.transform.parent = parent.transform;

                    currentObject = parent;

                    name.Remove(name.Last());
                }
            }
        }       

        /// <summary>
        /// Clear the duplicate that comes from the import software
        /// </summary>
        private void ClearDuplicateGameObjects() {

            for(int i = 0; i < transform.childCount; i++)
                for(int j = i + 1; j < transform.childCount; j++)
                    if(transform.GetChild(i).name == transform.GetChild(j).name)
                        DestroyImmediate(transform.GetChild(j).gameObject);
        }

        /// <summary>
        /// Make all parts be children of the root object
        /// </summary>
        private void MakeChild() {

            List<GameObject> rootObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);

            foreach(GameObject go in rootObjects)
                if(go.GetComponent<MeshRenderer>() != null)
                    go.transform.parent = this.transform;
        }
    }
}