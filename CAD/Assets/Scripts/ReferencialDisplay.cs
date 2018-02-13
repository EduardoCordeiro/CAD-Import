using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

using CAD.Utility;

namespace CAD.Managers {

    public class ReferencialDisplay : MonoBehaviour {

        GameObject sphere;

        List<GameObject> placeholders;

        List<GameObject> temporaryGrouping;

        List<GameObject> assemblyList;

        string directoryPath = "Assets/Resources/Caracteristic Files/";

        public int numberOfFiles;

        public float distanceThreshold;

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            placeholders = new List<GameObject>();

            temporaryGrouping = new List<GameObject>();

            assemblyList = new List<GameObject>();

            // this will be changed to different objects
            numberOfFiles = this.GetComponentsInChildren<Transform>().Length;

            distanceThreshold = 0.5f;

            CollectPlaceholderData(numberOfFiles);
            
            // distance between two sphere is < threshshold
            CheckSphereThresholdDistance();

            UpdateObjectLists();
        }

        // Update is called once per frame
        void Update() {

        }

        /// <summary>
        /// As the name suggests
        /// </summary>
        void CheckSphereThresholdDistance() {

            foreach(GameObject placeholder in placeholders) {

                Vector3 placeholderPosition = placeholder.transform.position;

                // Temp list to store tempoerary objects
                List<GameObject> similarObjects = new List<GameObject>();

                foreach(GameObject otherPlaceholder in placeholders) {

                    // No self comparisons
                    if(otherPlaceholder.name == placeholder.name)
                        continue;

                    Vector3 otherPlaceholderPostion = otherPlaceholder.transform.position;

                    if(Vector3.Distance(placeholderPosition, otherPlaceholderPostion) < distanceThreshold) {

                        // Collapse the spheres that are in the vicinity
                        similarObjects.Add(otherPlaceholder);

                        // Create a new Sphere in the mid point between the two spheres
                        // Hide the 2 placeholders
                        // add them as children of the new object
                        // add to assembly list
                        // remove from placeholder list

                        /*GameObject midwayObjet = CreateGameObject(  sphere, 
                                                                    new Vector3((placeholderPosition.x + otherPlaceholderPostion.x) / 2,
                                                                                (placeholderPosition.y + otherPlaceholderPostion.y) / 2,
                                                                                (placeholderPosition.z + otherPlaceholderPostion.z) / 2));*/

                        //placeholder.transform.parent = midwayObjet.transform;
                        //placeholder.SetActive(false);

                        //otherPlaceholder.transform.parent = midwayObjet.transform;
                        //otherPlaceholder.SetActive(false);

                        temporaryGrouping.Add(placeholder);
                        temporaryGrouping.Add(otherPlaceholder);
                    }
                }

                // here I have a list of all objects that are close to the one I started with
                // which seems pointless, because the problem of > 2 still occurs and this does not detect it
            }
        }

        /// <summary>
        /// Simple function to encapsulate the change from list -> list
        /// </summary>
        /// <param name="placeholder"></param>
        void UpdateObjectLists() {

            foreach(GameObject temporary in temporaryGrouping) {

                //temporary.SetActive(false);
                temporary.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }

        /// <summary>
        /// Read data from file eventually, current dummy randoms
        /// </summary>
        /// <param name="numberOfFiles"></param>
        void CollectPlaceholderData(int numberOfFiles) {

            for(int i = 0; i < numberOfFiles; i++) {

                // JSON parsing
                /*string fileName = "dummy";

                Caracteristic carateristic = JsonUtility.FromJson<Caracteristic>(directoryPath + fileName + ".json");
                
                CreateGameObject(sphere, new Vector3(carateristic.shape, carateristic.position, carateristic.joint));

                */

                Vector3 position = new Vector3(Random.Range(0.0f, 2.0f), Random.Range(1.5f, 3.5f), Random.Range(0.0f, 2.0f));

                CreateGameObject(sphere, position);
            }

            // Add some fake spheres to hightlit the referencial
            CreateGameObject(sphere, new Vector3(2.0f, 0.0f, 0.0f));
            ColorGameObject(placeholders[placeholders.Count - 1], Color.red);

            CreateGameObject(sphere, new Vector3(0.0f, 2.0f, 0.0f));
            ColorGameObject(placeholders[placeholders.Count - 1], Color.green);

            CreateGameObject(sphere, new Vector3(0.0f, 0.0f, 2.0f));
            ColorGameObject(placeholders[placeholders.Count - 1], Color.blue);

            CreateGameObject(sphere, new Vector3(2.0f, 2.0f, 2.0f));
            ColorGameObject(placeholders[placeholders.Count - 1], Color.black);
        }

        GameObject CreateGameObject(GameObject gameObject, Vector3 position) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);
            placeholder.name = gameObject.name + position.ToString();

            placeholders.Add(placeholder);

            return placeholder;
        }

        void ColorGameObject(GameObject gameObject, Color color) {

            gameObject.GetComponent<Renderer>().material.color = color;
        }

        Vector3 CalculatePosition(string[] values) {

            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

        // Substitute the code in OnDrawGizmos here
        void DrawReferencial() {
            
        }

        // Refactor this code to DrawReferencial
        void OnDrawGizmos() {

            Gizmos.color = Color.red;

            Gizmos.DrawLine(Vector3.zero, new Vector3(2.0f, 0.0f, 0.0f));

            Gizmos.color = Color.green;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0.0f, 2.0f, 0.0f));

            Gizmos.color = Color.blue;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0.0f, 0.0f, 2.0f));
        }
    }
}