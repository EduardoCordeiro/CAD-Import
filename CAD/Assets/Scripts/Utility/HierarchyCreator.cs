using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;

namespace CAD.Utility {

    public class HierarchyCreator : MonoBehaviour {

        public static HierarchyCreator instance { get; private set; }

        // Use this for initialization
        void Start() {

            instance = this;
        }

        // Update is called once per frame
        void Update() {

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

                List<string> name = c.name.Split('.').ToList();

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