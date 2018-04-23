using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Support;
using CAD.Support;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Actions {

    public class GazeSelection : MonoBehaviour {

        /// <summary>
        /// HitInfo property gives access to information at the object being gazed at, if any.
        /// </summary>
        public static RaycastHit currentHit { get; private set; }

        /// <summary>
        /// Hit from previous frame
        /// </summary>
        public RaycastHit oldHit { get; private set; }

        /// <summary>
        /// Boolean for determining if we are hitting an object or not
        /// </summary>
        public bool hittingObject { get; private set; }

        /// <summary>
        /// Query Object. Right now its flange-15
        /// </summary>
        public GameObject queryAssembly;

        /// <summary>
        /// Clone of the Query Object. Right now its flange-15
        /// It is necessary to compare the query model with itself
        /// </summary>
        public GameObject queryClone;

        /// <summary>
        /// Draw the Gaze ray
        /// </summary>
        public bool DebugDrawRay;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

           if (ReferencialDisplay.phase == Phase.CollectionSelection || ReferencialDisplay.phase == Phase.AssemblySelection) {

                Gaze();
            }
            else if(ReferencialDisplay.phase == Phase.AssemblyComparision) {

                AssemblyComparision();
            }

            if(DebugDrawRay)
                Debug.DrawRay(this.transform.position, this.transform.forward, Color.cyan);            
        }

        public void Gaze() {

            RaycastHit hitInfo;

            // If the Raycast has succeeded and hit a sphere
            // hitInfo's point represents the position being gazed at
            // hitInfo's collider GameObject represents the assembly being gazed at
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f)) {

                if(hitInfo.collider.tag == "Assemblies") {

                    hittingObject = true;

                    currentHit = hitInfo;

                    // Timer > 2 seconds, select Object
                    // here we will wait for 1-2 seconds and then select the object
                    StartCoroutine(GazeConfirmation());

                    // Old Hit Information
                    oldHit = hitInfo;
                }
                // do not do anything if we are not hitting an assembly

            } else
                hittingObject = false;
        }

        /// <summary>
        /// Selecting a Sphere
        /// </summary>
        public void SphereCollision() {

            if (currentHit.collider == oldHit.collider && hittingObject)
            {
                Debug.Log("Sphere collision: " + currentHit.transform.position);
                // Disable the Assembly and the Spheres
                ToggleAssemblies(false);
                ToggleSpheres(false);

                // Call the DispayAssembly Script
                var assembliesToDispay = currentHit.collider.GetComponent<DisplayAssembly>();
                // If only one assembly was hit, we are ready to compare the two [returned and query]
                if (assembliesToDispay == null)
                {
                    Debug.Log("Posizione colpita: " + currentHit.collider.transform.position);
                    ReferencialDisplay.phase = Phase.AssemblyComparision;
                    AssemblyCollision();
                }
                else
                {
                    int numberOfAssemblies = currentHit.collider.GetComponent<DisplayAssembly>().DisplayAssemblies();
                    ReferencialDisplay.phase = Phase.AssemblySelection;
                }
            }
        }

        /// <summary>
        /// Selecting an assembly
        /// </summary>
        void AssemblyCollision() {

            if(currentHit.collider == oldHit.collider && hittingObject) {

                ToggleAssemblies(false);
                // Re-enable the assemblie we want
                currentHit.collider.gameObject.SetActive(true);
                Debug.Log("Sfera colpita " + currentHit.transform.position);
                queryAssembly.SetActive(true);

                // TODO, change the query object, because when we want to compare the 15 to the 15, only 1 object will be shown

                ReferencialDisplay.phase = Phase.AssemblyComparision;
            }
        }

        void AssemblyComparision() {

            // Display the query assembly next to the one we want. With an offset in X for now
            queryAssembly.transform.position = currentHit.collider.transform.position + new Vector3(-0.5f, 0.0f, 0.0f);
            if (currentHit.collider.name == queryAssembly.name)
            {
                queryClone =
                    Instantiate(currentHit.transform.gameObject,
                        currentHit.collider.transform.position - new Vector3(-0.5f, 0.0f, 0.0f),
                        currentHit.collider.transform.rotation) as GameObject;
            }

            CompareAssemblies.instance.ParseLabels(currentHit.collider.name);
            // Parse the labels and color the objects and COLOR the Assemblies [not the best method]
            ReferencialDisplay.phase = Phase.AssemblyColorMatching;
        }

        IEnumerator GazeConfirmation() {

            yield return new WaitForSeconds(2);

            if (ReferencialDisplay.phase == Phase.CollectionSelection)
            {
                ReferencialDisplay.whereAssemblyComparisonComeFrom = Phase.CollectionSelection;
                SphereCollision();
             
            }
            else if (ReferencialDisplay.phase == Phase.AssemblySelection)
            {
                ReferencialDisplay.whereAssemblyComparisonComeFrom = Phase.AssemblySelection;
                AssemblyCollision();
            }
        }

        public void ToggleSpheres(bool value) {

            List<GameObject> rootObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);

            foreach(GameObject go in rootObjects)
                if(go.GetComponent<DisplayAssembly>() != null) {
                    if(value)
                        go.SetActive(true);
                    else
                        go.SetActive(false);
                }
        }

        public void ToggleAssemblies(bool value) {

            Transform assemblies = FindObjectOfType<CompareAssemblies>().transform;
            Debug.Log("Mostro assemblati " + value.ToString());
            foreach(Transform child in assemblies) {

                if(child.GetComponent<HierarchyCreator>() != null) {
                    // Only Display the comparison object and the query
                    // Can be change to setActive(false) all and setActive(true) those two
                    if(value)
                        child.gameObject.SetActive(true);
                    else
                        child.gameObject.SetActive(false);
                }
            }
        }
        
    }
}