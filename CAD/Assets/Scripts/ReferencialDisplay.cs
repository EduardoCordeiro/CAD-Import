using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using System.IO;
using System.Linq;

using CAD.Utility;
using System;

namespace CAD.Managers {

    public class ReferencialDisplay : MonoBehaviour {

        GameObject sphere;

        // placeholder for input
        List<GameObject> sphereRepresentationList;

        // List of all the assemblies in the scene
        List<GameObject> assemblyList;

        Dictionary<GameObject, Dictionary<GameObject, float>> distanceIssues = new Dictionary<GameObject, Dictionary<GameObject, float>>();

        string directoryPath = "Assets/Resources/Caracteristic Files/";

        public float threshhold = 2.0f;

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            sphereRepresentationList = new List<GameObject>();

            assemblyList = new List<GameObject>();

            // Add all assemblies as children of this object
            MakeChild();

            // Read the data from json for each assembly
            CollectAssemblyData();
            
            // distance between two sphere is < threshshold
            //CheckSphereThresholdDistance();

            // Add some fake spheres to hightlight the referencial
            CreateGameObject(sphere, new Vector3(2.0f, 0.0f, 0.0f));
            ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.red);

            CreateGameObject(sphere, new Vector3(0.0f, 2.0f, 0.0f));
            ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.green);

            CreateGameObject(sphere, new Vector3(0.0f, 0.0f, 2.0f));
            ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.blue);

            CreateGameObject(sphere, new Vector3(2.0f, 2.0f, 2.0f));
            ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.black);

            // Calculate the distances between the assemblies
            CalculateDistancesTable();

            ResolveDistanceProblems();

            UpdateObjectLists();
        }

        // Update is called once per frame
        void Update() {

        }

        /// <summary>
        /// Make all assemblies be children of this object
        /// </summary>
        private void MakeChild() {

            List<GameObject> rootObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);

            foreach(GameObject go in rootObjects)
                if(go.GetComponent<HierarchyCreator>() != null)
                    go.transform.parent = this.transform;
        }


        /// <summary>
        /// Read data from json files
        /// </summary>
        void CollectAssemblyData() {

            // JSON parsing
            // change from 0 --> i, when we have all the objects
            StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Caracteristic Files/" + this.transform.GetChild(0).name + ".json");
            string jsonString = sr.ReadToEnd();
            jsonString = Utility.Utility.FixJson(jsonString);

            // Deserialize Json file
            Caracteristic[] caracteristics = JsonHelper.FromJson<Caracteristic>(jsonString);

            // Order the List
            List<Caracteristic> sortedCaracteristics = caracteristics.OrderByDescending(o => o.localMeasure).ToList();
            // add this list to a list of lists that stores all the data

            // Save the highest local measure
            Caracteristic highestLocalMeasure = sortedCaracteristics[0];

            // Instanciate the sphere
            CreateGameObject(sphere, new Vector3(highestLocalMeasure.mustruct, highestLocalMeasure.muPos, highestLocalMeasure.mujoint));
        }

        /// <summary>
        /// As the name suggests
        /// </summary>
        void CheckSphereThresholdDistance() {

            foreach(GameObject placeholder in sphereRepresentationList) {

                Vector3 placeholderPosition = placeholder.transform.position;

                // Temp list to store tempoerary objects
                List<GameObject> similarObjects = new List<GameObject>();

                foreach(GameObject otherPlaceholder in sphereRepresentationList) {

                    // No self comparisons
                    if(otherPlaceholder.name == placeholder.name)
                        continue;

                    Vector3 otherPlaceholderPostion = otherPlaceholder.transform.position;

                    if(Vector3.Distance(placeholderPosition, otherPlaceholderPostion) < threshhold) {

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
                        
                    }
                }

                // here I have a list of all objects that are close to the one I started with
                // which seems pointless, because the problem of > 2 still occurs and this does not detect it
            }
        }

        /// <summary>
        /// This can be change to not store the distance at all, just a Dict<GameObject, List<GameObject>>
        /// </summary>
        void CalculateDistancesTable() {
            
            for(int i = 0; i < sphereRepresentationList.Count; i++) {

                Dictionary<GameObject, float> distances = new Dictionary<GameObject, float>();

                // Start in i + 1, as i == j
                for(int j = i + 1; j < sphereRepresentationList.Count; j++) {

                    // Only calculate distances for different objects
                    if(sphereRepresentationList[i].name != sphereRepresentationList[j].name) { 

                        float distance = Vector3.Distance(sphereRepresentationList[i].transform.position, sphereRepresentationList[j].transform.position);

                        // Only add problematic objects
                        if(distance < threshhold)
                            distances.Add(sphereRepresentationList[j], distance);
                    }
                }
                distanceIssues.Add(sphereRepresentationList[i], distances);
            }

            Debug.Log("Testing Distance Table" + sphereRepresentationList.Count);

            foreach(KeyValuePair<GameObject, Dictionary<GameObject, float>> issues in distanceIssues) {

                Debug.Log("Game Object Name = " + issues.Key);

                foreach(KeyValuePair<GameObject, float> distances in issues.Value) {

                    Debug.Log("Game Object = " + distances.Key + " :: distance = " + distances.Value);
                }
            }
        }

        void ResolveDistanceProblems() {

            foreach(KeyValuePair<GameObject, Dictionary<GameObject, float>> issues in distanceIssues) {

                if(issues.Value.Count == 0)
                    continue;

                foreach(Dictionary<GameObject, float> issue in distanceIssues.Values) {


                }
            }
        }

        /// <summary>
        /// Simple function to encapsulate the change from list -> list
        /// </summary>
        /// <param name="placeholder"></param>
        void UpdateObjectLists() {

            foreach(GameObject temporary in distanceIssues.Keys) {

                //temporary.SetActive(false);
                temporary.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }

        GameObject CreateGameObject(GameObject gameObject, Vector3 position) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);
            placeholder.name = gameObject.name + position.ToString();

            sphereRepresentationList.Add(placeholder);

            assemblyList.Add(placeholder);

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