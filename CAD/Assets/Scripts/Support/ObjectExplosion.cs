using System.Collections.Generic;
using CAD.Actions;
using UnityEngine;

namespace Assets.Scripts.Support {

    public class ObjectExplosion : Action {

        public int depthLevel = 0;
        
        // Use this for initialization
        void Start() {

            partList = new List<GameObject>();

            // Rework part Lists afer meeting
            partList = CreatePartList(depthLevel);

            initialPositions = new List<Vector3>();

            SaveStartingPositions(partList);
        }

        // Update is called once per frame
        void Update() {

            if(Input.GetKeyDown(KeyCode.F))
                Explosion(2.0f, partList);

            //if(Input.GetKeyDown(KeyCode.G))
            //    CircleExplosion(2.0f, partList);

            //if(Input.GetKeyDown(KeyCode.R))
            //    ReverseExplosion(partList);
        }

        void SaveStartingPositions(List<GameObject> partList) {

            foreach(GameObject part in partList)
                initialPositions.Add(part.transform.position);
        }

        List<GameObject> CreatePartList(int depthLevel) {

            List<GameObject> partList = new List<GameObject>();

            for(int i = 0; i < transform.childCount; i++) {

                Transform child = transform.GetChild(i);

                partList.Add(child.gameObject);

                // Adding Grand children if Depth Level > 0
                // Add Recursiveness here for the new depth levels (maybe change the function itself)
                if(depthLevel > 0 && child.childCount > 1)
                    for(int j = 0; j < child.childCount; j++)
                        partList.Add(child.GetChild(j).gameObject);
            }

            return partList;
        }

        /// <summary>
        /// Sphere Explosion
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="partList"></param>
        void Explosion(float radius, List<GameObject> partList) {

            Vector3 center = transform.position;

            int numberOfParts = partList.Count;

            int counter = 0;

            foreach(GameObject part in partList) {

                float x = center.x + radius * Mathf.Cos(2 * Mathf.PI * counter / numberOfParts) * Mathf.Sin(Mathf.PI * counter / numberOfParts);
                float y = center.y + radius * Mathf.Sin(2 * Mathf.PI * counter / numberOfParts) * Mathf.Sin(Mathf.PI * counter / numberOfParts);
                float z = center.z + radius * Mathf.Sin(Mathf.PI * counter / numberOfParts);

                /*
                x=origin.x+radius*cos(rotation.y)*cos(rotation.x)
                y=origin.y+radius*sin(rotation.x)
                z=origin.z+radius*sin(rotation.y)*cos(rotation.x)
                */

                part.transform.position = new Vector3(x, y, z);

                counter++;
            }
        }

        /// <summary>
        /// Circle Explosion
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="partList"></param>
        /// <param name="zPlane"></param>
        public void CircleExplosion(float radius, float zPlane, GameObject entireAssembly, ref List<Vector3> positionPartsBeforeTranformation, ref List<Quaternion> rotationPartsBeforeTranformation) {

            positionPartsBeforeTranformation = new List<Vector3>();
            rotationPartsBeforeTranformation = new List<Quaternion>();

            Vector3 center = entireAssembly.transform.position;
            int numberOfParts = entireAssembly.transform.childCount;
            int counter = 0;

            foreach(Transform part in entireAssembly.transform) {
                positionPartsBeforeTranformation.Add(part.position);
                rotationPartsBeforeTranformation.Add(part.rotation);

                float angle = 2 * Mathf.PI * counter / numberOfParts;

                float x = center.x + radius * Mathf.Cos(angle);
                float y = center.y + radius * Mathf.Sin(angle);

                part.gameObject.SetActive(true);
                part.transform.position = new Vector3(x, y, zPlane);
                Debug.Log("Piano frontale " + zPlane);
                counter++;
            }
        }

        public void ReverseExplosion(GameObject entireAssembly, List<Vector3> partsPositionBeforeTranformation, List<Quaternion> partsRotationBeforeTranformation) {

            int counter = 0;
            for (int i = 0; i < entireAssembly.transform.childCount; i++)
            {
                var part = entireAssembly.transform.GetChild(i);
                part.gameObject.SetActive(true);
                part.transform.position = partsPositionBeforeTranformation[i];
                part.transform.rotation = partsRotationBeforeTranformation[i];
            }
        }
    }
}