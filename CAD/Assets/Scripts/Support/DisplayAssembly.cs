using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Actions;
using UnityEngine;

namespace CAD.Support {

    public class DisplayAssembly : MonoBehaviour {

        public List<GameObject> assembliesList;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void StoreAssemblies(List<GameObject> assemblies) {

            this.assembliesList = new List<GameObject>(assemblies);
        }

        public int DisplayAssemblies(Vector3 hitPosition) {

            // Player
            Vector3 playerPosition = new Vector3(0.0f, 1.0f, 0.0f);

            // This assembly
            Vector3 newPosition = playerPosition + transform.TransformDirection(Vector3.forward) * 0.7f;

            this.transform.position = newPosition;

            Vector3 offset = new Vector3(0.5f, 0.0f, 0.0f);

            foreach(GameObject assembly in assembliesList) {

                assembly.SetActive(true);

                assembly.transform.position = newPosition + offset;

                newPosition += offset;
            }

            return assembliesList.Count;
        }

        public int DisplayAssemblies()
        {

            // Player
            //Vector3 playerPosition = new Vector3(0.0f, 1.5f, 0.0f);
            Vector3 playerPosition = GazeSelection.currentHit.transform.position;
            // This assembly
            Vector3 newPosition = playerPosition - new Vector3(1.0f, 0.0f, 0.0f); //+ transform.TransformDirection(Vector3.forward) * 0.7f;

            this.transform.position = newPosition;

            Vector3 offset = new Vector3(0.5f, 0.0f, 0.0f);

            Color greyColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            foreach (GameObject assembly in assembliesList)
            {
                assembly.SetActive(true);

                assembly.transform.position = newPosition + offset;

                newPosition += offset;

                foreach (Renderer variableName in assembly.transform.GetComponentsInChildren<Renderer>())
                {
                    variableName.material.color = greyColor;
                }
                
            }

            return assembliesList.Count;
        }
    }
}