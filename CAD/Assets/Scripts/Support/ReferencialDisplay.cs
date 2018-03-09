using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;
using System.Linq;

using CAD.Utility;

namespace CAD.Support {

    public class ReferencialDisplay : MonoBehaviour {

        public static ReferencialDisplay instance;
        
        GameObject sphere;

        // placeholder for input
        public List<GameObject> sphereRepresentationList;


        // placeholder to be visualized
        public List<GameObject> visibleSphereList;

        // Storing the data for the assemblies that are too close to eachother
        Dictionary<GameObject, List<GameObject>> distanceIssues = new Dictionary<GameObject, List<GameObject>>();

        // List of Json Data
        public Dictionary<GameObject, List<Caracteristic>> caracteristicsList { get; private set; }

        private float threshhold;

        public bool DisplayReferencial;

        // Using an offset in Y to account for the height of the user
        private float heightOffset = 0.5f;

        private void Awake() {

            instance = this;
        }

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            sphereRepresentationList = new List<GameObject>();

            visibleSphereList = new List<GameObject>();

            caracteristicsList = new Dictionary<GameObject, List<Caracteristic>>();

            threshhold = 0.05f;

            // Add all assemblies as children of this object
            MakeChild();

            // Read the data from json for each assembly
            CollectAssemblyData();

            // Dummy sphere for Debug
            if(DisplayReferencial) {
                
                // Add some fake spheres to hightlight the referencial
                CreateGameObject(sphere, new Vector3(1.0f, 0.5f, 0.0f), Color.red);

                CreateGameObject(sphere, new Vector3(0.0f, 1.5f, 0.0f), Color.green);

                CreateGameObject(sphere, new Vector3(0.0f, 0.5f, 1.0f), Color.blue);
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

            for (int i = 0; i < transform.childCount; i++)
            {

                string objectName = this.transform.GetChild(i).name;

                // JSON parsing
                StreamReader sr =
                    new StreamReader(Application.dataPath + "/Resources/Caracteristic Files/" + objectName + ".json");

                string jsonString = sr.ReadToEnd();
                jsonString = JsonHelper.FixJson(jsonString);

                // Deserialize Json file
                Caracteristic[] caracteristics = JsonHelper.FromJson<Caracteristic>(jsonString);

                // Order the List
                List<Caracteristic> sortedCaracteristics = new List<Caracteristic>();

                switch (MeasureInformation.measureType)
                {
                    case MeasureInformation.MeasureType.Global:
                        sortedCaracteristics = caracteristics.OrderByDescending(o => o.globalMeasure).ToList();
                        print("Ordino globale");
                        break;
                    case MeasureInformation.MeasureType.Partial:
                        sortedCaracteristics = caracteristics.OrderByDescending(o => o.partialMeasure).ToList();
                        print("Ordino parziale");
                        break;
                    case MeasureInformation.MeasureType.Local:
                        sortedCaracteristics = caracteristics.OrderByDescending(o => o.localMeasure).ToList();
                        print("Ordino local");
                        break;
                }

                // add this list to a dictionary of lists to store all the data
                //caracteristicsList = new KeyValuePair<GameObject, List<Caracteristic>>(this.transform.GetChild(i).gameObject, sortedCaracteristics);
                caracteristicsList.Add(this.transform.GetChild(i).gameObject, sortedCaracteristics);

                // Save the highest local measure
                Caracteristic highestLocalMeasure = sortedCaracteristics[0];

                List<Caracteristic> v = caracteristicsList[this.transform.GetChild(i).gameObject];

                // Instanciate the sphere
                CreateGameObject(sphere,
                    new Vector3(highestLocalMeasure.mushape,
                        highestLocalMeasure.muPos + heightOffset,
                        highestLocalMeasure.mujoint),
                    objectName,
                    new List<GameObject>() {this.transform.GetChild(i).gameObject});
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

            Transform assembliesTranform = GameObject.Find("Assemblies").transform;

            foreach(KeyValuePair<GameObject, List<GameObject>> issues in distanceIssues) {

                if(issues.Value.Count == 0)
                    continue;

                Vector3 minPoint = issues.Key.transform.position;
                Vector3 maxPoint = issues.Key.transform.position;

                List<GameObject> assembliesList = new List<GameObject>();

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

                    // Set the sphere as false, as we are not intersecting it
                    issue.SetActive(false);
                    
                    // Get the Actual assembly, and not the sphere
                    assembliesList.Add(assembliesTranform.Find(issue.name).gameObject);
                }

                // Set the sphere as false, as we are not intersecting it
                issues.Key.SetActive(false);

                // Get the Actual assembly, and not the sphere, for the object we are comparing to
                assembliesList.Add(assembliesTranform.Find(issues.Key.name).gameObject);

                // using centroid as the new position
                var newSpere = CreateGameObject(sphere, centroid, assembliesList);
                visibleSphereList.Add(newSpere);
            }
        }

        /// <summary>
        /// Used for Dummy GameObjects
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        GameObject CreateGameObject(GameObject gameObject, Vector3 position, Color color) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);

            placeholder.name = gameObject.name + position.ToString();

            ColorGameObject(placeholder, color);

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

            placeholder.layer = 14;
            placeholder.tag = "Assemblies";

            sphereRepresentationList.Add(placeholder);

            return placeholder;
        }

        /// <summary>
        /// Used for sphere creation, no name is given
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        GameObject CreateGameObject(GameObject gameObject, Vector3 position, List<GameObject> assemblies) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);

            placeholder.name = gameObject.name + position.ToString();

            // Script that will display the assemblies
            placeholder.AddComponent<DisplayAssembly>();
            placeholder.GetComponent<DisplayAssembly>().StoreAssemblies(assemblies);

            // 14 is the Assemblies layer
            placeholder.layer = 14;
            placeholder.tag = "Assemblies";

            sphereRepresentationList.Add(placeholder);

            // Coloring for now
            ColorGameObject(placeholder, Color.magenta);

            return placeholder;
        }

        /// <summary>
        /// Simple wrapper to Color a gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="color"></param>
        void ColorGameObject(GameObject gameObject, Color color) {

            gameObject.GetComponent<Renderer>().material.color = color;
        }
    }
}