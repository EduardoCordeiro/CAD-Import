using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAD.Utility {

    public class HierarchyCreator : MonoBehaviour {

        public List<GameObject> partList;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void CreateHierarchy() {

            foreach(GameObject part in partList) {

                part.transform.parent = this.transform;
            }
        }
    }
}