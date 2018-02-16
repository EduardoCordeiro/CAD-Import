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

        Dictionary<GameObject, List<GameObject>> distanceIssues = new Dictionary<GameObject, List<GameObject>>();

        string directoryPath = "Assets/Resources/Caracteristic Files/";

        public float threshhold;

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            sphereRepresentationList = new List<GameObject>();

            assemblyList = new List<GameObject>();

            threshhold = 0.7f;

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

            // Dummy sphere for Debug
            CreateGameObject(sphere, new Vector3(0.6f, 1.2f, 1.2f));
            ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.white);

            CreateGameObject(sphere, new Vector3(0.8f, 1.1f, 0.99f));
            ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.white);

            // Calculate the distances between the assemblies
            CalculateDistancesTable();

            // Resolve Distance problems that arise
            ResolveDistanceProblems();
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
        /// This can be change to not store the distance at all, just a Dict<GameObject, List<GameObject>>
        /// </summary>
        void CalculateDistancesTable() {

            List<GameObject> temporaryList = new List<GameObject>();

            foreach(GameObject go in sphereRepresentationList)
                temporaryList.Add(go);

            GameObject currentSphere;
            int counter = 0;

            while(temporaryList.Count >= 0) {

                currentSphere = sphereRepresentationList[counter];

                List<GameObject> distances = new List<GameObject>();

                for(int i = 0; i < sphereRepresentationList.Count; i++) {

                    if(currentSphere.name != sphereRepresentationList[i].name) {

                        Debug.Log("Distance between" + currentSphere.name + " and " + sphereRepresentationList[i].name);

                        float distance = Vector3.Distance(currentSphere.transform.position, sphereRepresentationList[i].transform.position);

                        // Only add problematic objects
                        if(distance < threshhold) {

                            distances.Add(sphereRepresentationList[i]);

                            temporaryList.Remove(temporaryList[i]);

                            Debug.Log(sphereRepresentationList[i].name + "how many removes am i doing");
                        }                            
                    }
                }

                distanceIssues.Add(currentSphere, distances);

                counter++;
            }
            
            /*for(int i = 0; i < sphereRepresentationList.Count; i++) {

                List<GameObject> distances = new List<GameObject>();

                // Start in i + 1, as i == j
                for(int j = 0; j < sphereRepresentationList.Count; j++) {

                    // Only calculate distances for different objects
                    if(sphereRepresentationList[i].name != sphereRepresentationList[j].name) { 

                        float distance = Vector3.Distance(sphereRepresentationList[i].transform.position, sphereRepresentationList[j].transform.position);

                        // Only add problematic objects
                        if(distance < threshhold)
                            distances.Add(sphereRepresentationList[j]);
                    }
                }
                distanceIssues.Add(sphereRepresentationList[i], distances);
            }*/
        }

        void ResolveDistanceProblems() {

            Vector3 newSpherePosition = Vector3.zero; 
            
            // Color the principal gameObject too
            distanceIssues.Keys.First().GetComponent<Renderer>().material.color = Color.magenta;

            Vector3 centroid = Vector3.zero;

            foreach(List<GameObject> issues in distanceIssues.Values) {

                if(issues.Count == 0)
                    continue;

                Debug.Log("and here");

                Vector3 minPoint = distanceIssues.Keys.First().transform.position;
                Vector3 maxPoint = distanceIssues.Keys.First().transform.position;

                foreach(GameObject issue in issues) {
                    Debug.Log(issue.name + "twice and then once");
                    Vector3 pos = issue.transform.position;

                    if(pos.x < minPoint.x)
                        minPoint.x = pos.x;
                    if(pos.x > maxPoint.x)
                        maxPoint.x = pos.x;
                    if(pos.y < minPoint.y)
                        minPoint.y = pos.y;
                    if(pos.y > maxPoint.y)
                        maxPoint.y = pos.y;
                    if(pos.z < minPoint.z)
                        minPoint.z = pos.z;
                    if(pos.z > maxPoint.z)
                        maxPoint.z = pos.z;                    

                    centroid = minPoint + 0.5f * (maxPoint - minPoint);

                    issue.GetComponent<Renderer>().material.color = Color.magenta;
                }

                newSpherePosition = centroid;

                CreateGameObject(sphere, newSpherePosition);
            }            
        }

        GameObject CreateGameObject(GameObject gameObject, Vector3 position) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);
            placeholder.name = gameObject.name + position.ToString();

            sphereRepresentationList.Add(placeholder);

            return placeholder;
        }

        void ColorGameObject(GameObject gameObject, Color color) {

            gameObject.GetComponent<Renderer>().material.color = color;
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