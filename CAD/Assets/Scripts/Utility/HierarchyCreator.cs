using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            ClearDuplicateGameObjects();

            Hierarchy();
        }

        private void Hierarchy() {

            List<Transform> children = new List<Transform>();

            foreach(Transform child in this.transform)
                children.Add(child);

            foreach(Transform c in children) {

                List<string> name = c.name.Split(new char[0]).ToList();

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

        public void AssignRootName(string rootName) {

            //C:/ Users / Eduardo / Desktop / flange - coupling - 15_strutturaModificata / STLforX3D /

            string[] split = rootName.Split('/');

            this.name = split[split.Length - 3];
        }
    }
}