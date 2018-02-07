using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAD.Utility {

    public class HierarchyCreator : MonoBehaviour {

        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {

        }

        public void CreateHierarchy() {

        }

        public void ReadHierarchy() {

            // Remenber we ignore the deepest level (due to mesh creation and disasembly)
            int depthLevel = 0;

            if(transform.childCount > 0) {

                depthLevel++;

                List<GameObject> currentLevel = new List<GameObject>();

                for(int i = 0; i < transform.childCount; i++) {

                    currentLevel.Add(transform.GetChild(i).gameObject);
                }

            }
        }
    }
}