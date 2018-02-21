using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;
using System.Linq;

using CAD.Utility;

namespace CAD.Managers {

    public class ReferencialDisplay : MonoBehaviour {

        Vector3 referencialPosition;

        GameObject sphere;

        // placeholder for input
        List<GameObject> sphereRepresentationList;

        // List of all the assemblies in the scene
        List<GameObject> assemblyList;

        // Storing the data for the assemblies that are too close to eachother
        Dictionary<GameObject, List<GameObject>> distanceIssues = new Dictionary<GameObject, List<GameObject>>();

        // List of Json Data
        List<List<Caracteristic>> caracteristicLists;

        string directoryPath = "Assets/Resources/Caracteristic Files/";

        private float threshhold;

        public bool debug;

        // Use this for initialization
        void Start() {

            referencialPosition = Camera.main.transform.position - Vector3.zero / 2;

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            sphereRepresentationList = new List<GameObject>();

            assemblyList = new List<GameObject>();

            caracteristicLists = new List<List<Caracteristic>>();

            threshhold = 0.05f;

            // Add all assemblies as children of this object
            MakeChild();

            // Read the data from json for each assembly
            CollectAssemblyData();

            // Dummy sphere for Debug
            if(debug) {
                
                // Add some fake spheres to hightlight the referencial
                /*CreateGameObject(sphere, new Vector3(1.0f, 0.0f, 0.0f));
                ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.red);

                CreateGameObject(sphere, new Vector3(0.0f, 1.0f, 0.0f));
                ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.green);

                CreateGameObject(sphere, new Vector3(0.0f, 0.0f, 1.0f));
                ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.blue);

                CreateGameObject(sphere, new Vector3(1.0f, 1.0f, 1.0f));
                ColorGameObject(sphereRepresentationList[sphereRepresentationList.Count - 1], Color.black);*/

                CreateGameObject(sphere, new Vector3(0.6f, .2f, .2f));

                CreateGameObject(sphere, new Vector3(0.3f, 0.0f, 1.0f));

                CreateGameObject(sphere, new Vector3(1.0f, 0.2f, 0.1f));

                CreateGameObject(sphere, new Vector3(1.0f, 0.3f, 0.1f));

                CreateGameObject(sphere, new Vector3(0.8f, 0.5f, 0.1f));
            }

            // Calculate the distances between the assemblies
            CalculateDistancesTable();

            // Resolve Distance problems
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

            for(int i = 0; i < transform.childCount; i++) {

                string objectName = this.transform.GetChild(i).name;

                // JSON parsing
                StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Caracteristic Files/" + objectName + ".json");

                string jsonString = sr.ReadToEnd();
                jsonString = JsonHelper.FixJson(jsonString);

                // Deserialize Json file
                Caracteristic[] caracteristics = JsonHelper.FromJson<Caracteristic>(jsonString);

                // Order the List
                List<Caracteristic> sortedCaracteristics = caracteristics.OrderByDescending(o => o.localMeasure).ToList();

                // add this list to a list of lists to store all the data
                caracteristicLists.Add(sortedCaracteristics);

                // Save the highest local measure
                Caracteristic highestLocalMeasure = sortedCaracteristics[0];

                // Instanciate the sphere
                CreateGameObject(   sphere, 
                                    new Vector3(highestLocalMeasure.mushape, 
                                                highestLocalMeasure.muPos, 
                                                highestLocalMeasure.mujoint), 
                                    objectName, 
                                    new List<GameObject>() { this.transform.GetChild(i).gameObject });
            }
        }

        /// <summary>
        /// This can be change to not store the distance at all, just a Dict<GameObject, List<GameObject>>
        /// </summary>
        void CalculateDistancesTable() {

            List<GameObject> temporaryList = new List<GameObject>();
            List<GameObject> removalList = new List<GameObject>();

            foreach(GameObject go in sphereRepresentationList)
                temporaryList.Add(go);

            GameObject currentSphere;
            int counter = 0;

            while(temporaryList.Count > 0) {

                currentSphere = temporaryList[0];

                List<GameObject> issues = new List<GameObject>();

                for(int i = 0; i < temporaryList.Count; i++) {

                    if(currentSphere.name != temporaryList[i].name) {

                        float distance = Vector3.Distance(currentSphere.transform.position, temporaryList[i].transform.position);

                        // Only add problematic objects
                        if(distance < threshhold) {

                            issues.Add(temporaryList[i]);

                            removalList.Add(temporaryList[i]);
                        }                            
                    }
                }

                // Add gameobject to issue list
                distanceIssues.Add(currentSphere, issues);

                // remove current object from temp list
                temporaryList.Remove(currentSphere);

                // remove all problematic objects from temp
                foreach(GameObject r in removalList)
                    temporaryList.Remove(r);

                // safe clear, not needed
                removalList.Clear();

                counter++;
            }
        }

        /// <summary>
        /// Generic function that for all the close assemblies calculates the new position for the sphere
        /// </summary>
        void ResolveDistanceProblems() {

            Vector3 centroid = Vector3.zero;

            foreach(KeyValuePair<GameObject, List<GameObject>> issues in distanceIssues) {

                if(issues.Value.Count == 0)
                    continue;

                Vector3 minPoint = issues.Key.transform.position;
                Vector3 maxPoint = issues.Key.transform.position;

                foreach(GameObject issue in issues.Value) {

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
                }

                List<GameObject> assembliesList = new List<GameObject>(issues.Value);
                assembliesList.Add(issues.Key);

                // using centroid as the new position
                CreateGameObject(sphere, centroid, assembliesList);
            }
        }

        /// <summary>
        /// Used for Dummy GameObjects
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        GameObject CreateGameObject(GameObject gameObject, Vector3 position) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);

            placeholder.name = gameObject.name + position.ToString();

            sphereRepresentationList.Add(placeholder);

            return placeholder;
        }

        /// <summary>
        /// Used for the initialization, when parsing the Json files of all the assemblies
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        GameObject CreateGameObject(GameObject gameObject, Vector3 position, string name, List<GameObject> assemblies) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);
            
            placeholder.name = name;

            // Script that will display the assemblies
            placeholder.AddComponent<DisplayAssembly>();
            placeholder.GetComponent<DisplayAssembly>().StoreAssemblies(assemblies);

            sphereRepresentationList.Add(placeholder);

            return placeholder;
        }

        GameObject CreateGameObject(GameObject gameObject, Vector3 position, List<GameObject> assemblies) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);

            placeholder.name = gameObject.name + position.ToString();

            // Script that will display the assemblies
            placeholder.AddComponent<DisplayAssembly>();
            placeholder.GetComponent<DisplayAssembly>().StoreAssemblies(assemblies);

            sphereRepresentationList.Add(placeholder);

            // Coloring for now
            ColorGameObject(placeholder, Color.magenta);

            return placeholder;
        }

        void ColorGameObject(GameObject gameObject, Color color) {

            gameObject.GetComponent<Renderer>().material.color = color;
        }
    }
}