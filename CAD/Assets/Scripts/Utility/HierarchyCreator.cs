using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;

namespace CAD.Utility {

    public class HierarchyCreator : MonoBehaviour {

        public static HierarchyCreator instance { get; private set; }

        //public string rootName;


        // Use this for initialization
        void Start() {

            instance = this;
        }

        // Update is called once per frame
        void Update() {

        }

        public void CreateHierarchy() {

            Debug.Log("CreateHierarchy()");

            MakeChild();

            ClearDuplicateGameObjects();

            Hierarchy();
        }

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

        private void ClearDuplicateGameObjects() {

            for(int i = 0; i < transform.childCount; i++)
                for(int j = i + 1; j < transform.childCount; j++)
                    if(transform.GetChild(i).name == transform.GetChild(j).name)
                        DestroyImmediate(transform.GetChild(j).gameObject);
        }

        private void MakeChild() {

            List<GameObject> rootObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);

            foreach(GameObject go in rootObjects)
                if(go.GetComponent<MeshRenderer>() != null)
                    go.transform.parent = this.transform;
        }

        // Really bad implementation until we decide on folder structure
        public void AssignRootName(string rootName) {

            //C:/ Users / Eduardo / Desktop / flange - coupling - 15_strutturaModificata / STLforX3D /

            string[] split = rootName.Split('/');

            this.name = split[split.Length - 3];
        }
    }
}