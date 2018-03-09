using System;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Support;
using CAD.Support;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

namespace Assets.Scripts.Actions
{
    class VoiceController : MonoBehaviour
    {
        [SerializeField]
        private string[] m_Keywords;

        private List<Vector3> originalParts;
        private KeywordRecognizer m_Recognizer;

        private int countAssembly = 0;
        private int maximumAssemblyNumber = 0;
        void Start()
        {
            m_Recognizer = new KeywordRecognizer(m_Keywords);
            m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
            m_Recognizer.Start();
        }

        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
            builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
            builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
            Debug.Log(builder.ToString());

            var targetObject = CompareAssemblies.instance.otherAssembly;
            var gazeSelcetion = GameObject.Find("CenterEyeAnchor").GetComponent<GazeSelection>();

            List<Transform> children = new List<Transform>();

            switch (args.text)
            {
                case "Nascondi":
                    break;
                case "Mostra":
                    break;
                case "Decomponi":
                    //if (ReferencialDisplay.phase == Phase.AssemblyExplosion)
                    {
                        DisplaySelectedSubassmbly(targetObject, countAssembly);
                        maximumAssemblyNumber = targetObject.transform.childCount;
                        Debug.Log("Subassembly da iterare " + maximumAssemblyNumber);
                        ReferencialDisplay.phase = Phase.AssemblyDecomposition;
                    }
                    break;
                case "Next":
                    if (ReferencialDisplay.phase == Phase.AssemblyDecomposition)
                    {
                        if (countAssembly == maximumAssemblyNumber - 1)
                        {
                            countAssembly = 0;
                        }
                        else
                        {
                            countAssembly++;
                        }
                        Debug.Log(countAssembly);
                        DisplaySelectedSubassmbly(targetObject, countAssembly);
                    }
                    break;

                case "Esplodi":
                    var zPlane = targetObject.gameObject.transform.position.z;
                    originalParts = GetComponent<ObjectExplosion>().CircleExplosion(0.7f, zPlane, CompareAssemblies.instance.otherAssembly);
                    ReferencialDisplay.phase = Phase.AssemblyExplosion;
                    break;
                case "Assembla":
                    if (ReferencialDisplay.phase == Phase.AssemblyExplosion ||
                        ReferencialDisplay.phase == Phase.AssemblyDecomposition)
                    {
                        GetComponent<ObjectExplosion>()
                            .ReverseExplosion(CompareAssemblies.instance.otherAssembly, originalParts);
                    }
                    ReferencialDisplay.phase = Phase.Done;
                    Debug.Log("Aggiornarto con " + ReferencialDisplay.phase);
                    break;
                case "Indietro":
                    Debug.Log("Comando INDIETRO con fase: " + ReferencialDisplay.phase);
                    switch (ReferencialDisplay.phase)
                    {
                        case Phase.None:
                            SceneManager.LoadScene("MeasureSelection");
                            break;
                        case Phase.AssemblySelection:
                            Scene loadLevel = SceneManager.GetActiveScene();
                            SceneManager.LoadScene(loadLevel.name);
                            break;
                        case Phase.AssemblyComparision:
                          //  if (ReferencialDisplay.phaseHistory[ReferencialDisplay.phaseHistory.Count - 3] == Phase.AssemblySelection)
                            {
                                Debug.Log("Sono nell'ultima fase IF");

                                gazeSelcetion.ToggleAssemblies(false);
                                var spheres = ReferencialDisplay.instance.sphereRepresentationList;
                                //var lastSphereGameObject = gazeSelcetion.oldHit.collider.name;
                                var lastSphere = spheres.Find(s => s.GetComponent<DisplayAssembly>().assembliesList.Find(obj => obj.name == CompareAssemblies.instance.otherAssembly.name));
                                var numberOfObjects = lastSphere.GetComponent<DisplayAssembly>().assembliesList;
                                Debug.Log("Vengo dalla sfera: " + lastSphere.gameObject.name + " che contiene " + numberOfObjects);
                                lastSphere.GetComponent<DisplayAssembly>().DisplayAssemblies();
                                ReferencialDisplay.phase = Phase.AssemblySelection;
                            }
                            //else if (ReferencialDisplay.phaseHistory[ReferencialDisplay.phaseHistory.Count - 3] == Phase.None)
                            //{
                            //    Debug.Log("Sono nell'ultima fase ELSE");

                            //    Scene loadPreviousLevel = SceneManager.GetActiveScene();
                            //    SceneManager.LoadScene(loadPreviousLevel.name);
                            //}
                            break;
                        case Phase.Done:
                            Debug.Log("Sono nella fase DONE");

                            //var count = 0;
                            //foreach (Phase phase in ReferencialDisplay.phaseHistory)
                            //{
                            //    Debug.Log(count + " " + phase.ToString());
                            //    count++;
                            //}

                            //if (ReferencialDisplay.phaseHistory[ReferencialDisplay.phaseHistory.Count - 3] == Phase.AssemblySelection)
                            {
                                var spheres = ReferencialDisplay.instance.visibleSphereList;

                                Debug.Log("Sono nell'ultima fase di DONE -- IF");

                                foreach (GameObject obj in spheres)
                                {
                                    Debug.Log(obj.gameObject.name);
                                }

                                gazeSelcetion.ToggleAssemblies(false);

                                //var lastSphereGameObject = gazeSelcetion.oldHit.collider.name;
                                var lastSphere = spheres.Find(s => s.GetComponent<DisplayAssembly>().assembliesList.Find(obj => obj.name == CompareAssemblies.instance.otherAssembly.name));
                                var numberOfObjects = lastSphere.GetComponent<DisplayAssembly>().DisplayAssemblies();
                                Debug.Log("Vengo dalla sfera: " + lastSphere.gameObject.name);
                                ReferencialDisplay.phase = Phase.AssemblySelection;

                                //.DisplayAssemblies();

                                ReferencialDisplay.phase = Phase.AssemblySelection;
                            }
                            //else if (ReferencialDisplay.phaseHistory[ReferencialDisplay.phaseHistory.Count - 3] == Phase.None)
                            //{
                            //    Debug.Log("Sono nell'ultima fase di DONE -- ELSE IF");

                            //    Scene loadPreviousLevel = SceneManager.GetActiveScene();
                            //    SceneManager.LoadScene(loadPreviousLevel.name);
                            //}
                            //else
                            //{
                            //    Debug.Log("Sono nell'ultima fase di DONE -- ELSE");
                            //}
                            break;
                        
                        case Phase.AssemblyDecomposition:
                            zPlane = targetObject.gameObject.transform.position.z;
                            originalParts = GetComponent<ObjectExplosion>().CircleExplosion(0.7f, zPlane, CompareAssemblies.instance.otherAssembly);
                            ReferencialDisplay.phase = Phase.AssemblyExplosion;
                            break;
                        default:
                            Debug.Log(ReferencialDisplay.phase);
                            break;
                    }
                    break;
            }
        }

        private void DisplaySelectedSubassmbly(GameObject targetObject, int countAssembly)
        {
            foreach (Transform child in targetObject.transform)
            {
                child.gameObject.SetActive(false);
            }

            // creea il bounding box per il grasp con il leap motion
            // ATTENZIONE: non è centrato correttamente sull'oggetto!

            Debug.Log("Mostrare" + targetObject.transform.GetChild(countAssembly).name);
            var visibleSubAss = targetObject.transform.GetChild(countAssembly).gameObject;
            visibleSubAss.SetActive(true);
            var boxCollider = visibleSubAss.AddComponent<BoxCollider>();
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bounds.center = Vector3.zero;

            if (bounds.extents == Vector3.zero)
                bounds = visibleSubAss.GetComponent<MeshFilter>().mesh.bounds;
            
            bounds.Encapsulate(visibleSubAss.GetComponent<MeshFilter>().mesh.bounds);

            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;
            Debug.Log("Center " + bounds.center.x + " " + bounds.center.y + " " + bounds.center.z);
            Debug.Log("Size " + bounds.size.x + " " + bounds.size.y + " " + bounds.size.z);

            // Add the leap motion script by code
            var interactionBehaviour = visibleSubAss.AddComponent<InteractionBehaviour>();
            interactionBehaviour.ignoreContact = true;
            interactionBehaviour.moveObjectWhenGrasped = true;
            interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Inherit;
            interactionBehaviour.graspHoldWarpingEnabled__curIgnored = false;

            //interactionBehaviour.OnGraspEnd = () => GameObject.Find(targetObject.name).GetComponent<GestureInteraction>().StopGrasp(targetObject);
            //System.Action  =
            //  GameObject.Find("Assemblies").GetComponent<GestureInteraction>().StopGrasp;


            // Rigidbody
            var rigidBody = visibleSubAss.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        public void KatiaStopObject(GameObject gameObject)
        {

            print("Stopped grasp");

            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
