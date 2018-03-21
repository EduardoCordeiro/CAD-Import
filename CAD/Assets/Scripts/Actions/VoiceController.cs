using System;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Support;
using CAD.Support;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System.Linq;
using Collider = Assets.Scripts.Support.Collider;

namespace Assets.Scripts.Actions
{
    class VoiceController : MonoBehaviour
    {
        [SerializeField]
        private string[] m_Keywords;

        private List<Vector3> originalPositionParts;
        private List<Quaternion> originalRotationParts;

        private KeywordRecognizer m_Recognizer;

        private int countAssembly = 0;
        private int maximumAssemblyNumber = 0;
        private float _colorA;

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
                    if (ReferencialDisplay.phase == Phase.AssemblyColorMatching)
                    {
                        originalPositionParts = new List<Vector3>();
                        originalRotationParts = new List<Quaternion>();
                        foreach (Transform part in targetObject.transform)
                        {
                            originalPositionParts.Add(part.position);
                            originalRotationParts.Add(part.rotation);
                        }

                        DisplaySelectedSubassmbly(targetObject, countAssembly);
                        var visibleSubAss = targetObject.transform.GetChild(countAssembly).gameObject;
                        if (visibleSubAss.transform.childCount == 0)
                        {
                            AllowPartGrasping(targetObject, visibleSubAss);
                        }
                        else
                        {
                            AllowComponentGrasping(targetObject, visibleSubAss);
                        }
                        maximumAssemblyNumber = targetObject.transform.childCount;
                        Debug.Log("Subassembly da iterare " + maximumAssemblyNumber);
                        ReferencialDisplay.phase = Phase.AssemblyDecomposition;
                    }
                    break;
                case "Avanti":
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
                        var visibleSubAss = targetObject.transform.GetChild(countAssembly).gameObject;
                        var part = visibleSubAss.gameObject;
                        if (part.transform.childCount == 0)
                        {
                            AllowPartGrasping(targetObject, part);
                        }
                        else
                        {
                            AllowComponentGrasping(targetObject, part);
                        }
                    }
                    break;
                case "Esplodi":
                    if (ReferencialDisplay.phase == Phase.AssemblyColorMatching)
                    {

                        var zPlane = targetObject.gameObject.transform.position.z;
                        GetComponent<ObjectExplosion>()
                            .CircleExplosion(0.7f, zPlane, CompareAssemblies.instance.otherAssembly,
                                ref originalPositionParts, ref originalRotationParts);

                        foreach (Transform partTransform in targetObject.transform)
                        {
                            var part = partTransform.gameObject;
                            if (partTransform.childCount == 0)
                            {
                                AllowPartGrasping(targetObject, part);
                            }
                            else
                            {
                                AllowComponentGrasping(targetObject, part);
                            }
                        }
                        ReferencialDisplay.phase = Phase.AssemblyExplosion;
                    }
                    break;
                case "Assembla":
                    if (ReferencialDisplay.phase == Phase.AssemblyExplosion || ReferencialDisplay.phase == Phase.AssemblyDecomposition)
                    {
                        GetComponent<ObjectExplosion>()
                            .ReverseExplosion(CompareAssemblies.instance.otherAssembly, originalPositionParts, originalRotationParts);

                        foreach (Transform child in targetObject.transform)
                        {

                                var material = child.gameObject.GetComponent<Renderer>().material;
                                StandardShaderUtils.ChangeRenderMode(ref material, StandardShaderUtils.BlendMode.Opaque);
                                child.gameObject.GetComponent<Renderer>().material = material;
                                Debug.Log(string.Format("Trasparenza di {0}", child.name));

                                //child.gameObject.SetActive(false);
                        }
                    }

                    ReferencialDisplay.phase = Phase.AssemblyColorMatching;
                    Debug.Log("Aggiornarto con " + ReferencialDisplay.phase);
                    break;
                case "Indietro":
                    Debug.Log("Comando INDIETRO con fase: " + ReferencialDisplay.phase);
                    switch (ReferencialDisplay.phase)
                    {
                        case Phase.CollectionSelection:
                            SceneManager.LoadScene("MeasureSelection");
                            break;
                        case Phase.AssemblySelection:
                            Scene loadLevel = SceneManager.GetActiveScene();
                            SceneManager.LoadScene(loadLevel.name);
                            break;
                          
                        case Phase.AssemblyColorMatching:
                            if (ReferencialDisplay.whereAssemblyComparisonComeFrom == Phase.AssemblySelection)
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

                                if (gazeSelcetion.queryClone != null)
                                {
                                    Destroy(gazeSelcetion.queryClone);
                                }
                                var numberOfObjects = lastSphere.GetComponent<DisplayAssembly>().DisplayAssemblies();
                                Debug.Log("Vengo dalla sfera: " + lastSphere.gameObject.name);
                                ReferencialDisplay.phase = Phase.AssemblySelection;

                                //.DisplayAssemblies();

                                ReferencialDisplay.phase = Phase.AssemblySelection;
                            }
                            else if (ReferencialDisplay.whereAssemblyComparisonComeFrom == Phase.CollectionSelection)
                            {
                                Debug.Log("Sono nell'ultima fase di DONE -- ELSE IF");

                                Scene loadPreviousLevel = SceneManager.GetActiveScene();
                                SceneManager.LoadScene(loadPreviousLevel.name);
                            }
                            else
                            {
                                Debug.Log("Sono nell'ultima fase di DONE -- ELSE");
                            }
                            break;
                        case Phase.AssemblyDecomposition:
                            //zPlane = targetObject.gameObject.transform.position.z;
                            //originalPositionParts = GetComponent<ObjectExplosion>().CircleExplosion(0.7f, zPlane, CompareAssemblies.instance.otherAssembly);
                            //ReferencialDisplay.phase = Phase.AssemblyExplosion;
                            break;
                        default:
                            Debug.Log(ReferencialDisplay.phase);
                            break;
                    }
                    break;
            }
        }

        public void DisplaySelectedSubassmbly(GameObject targetObject, int countAssembly)
        {
            
            var visibleSubAss = targetObject.transform.GetChild(countAssembly).gameObject;

            foreach (Transform child in targetObject.transform)
            {
                if (child.gameObject != visibleSubAss)
                {
                    var material = child.gameObject.GetComponent<Renderer>().material;
                    var originalColor = material.color;
                    var newColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
                    var color = child.gameObject.GetComponent<Renderer>().material.color = newColor;
                    Debug.Log(string.Format("{0}: {1}, {2}, {3}, A: {4}", child.name, color.r, color.g, color.b, color.a));

                    StandardShaderUtils.ChangeRenderMode(ref material, StandardShaderUtils.BlendMode.Fade);
                    child.gameObject.GetComponent<Renderer>().material = material;
                    Debug.Log(string.Format("Trasparenza di {0}", child.name));

                    //child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                    var material = child.gameObject.GetComponent<Renderer>().material;
                    var originalColor = material.color;
                    var newColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
                    var color = child.gameObject.GetComponent<Renderer>().material.color = newColor;
                    StandardShaderUtils.ChangeRenderMode(ref material, StandardShaderUtils.BlendMode.Opaque);
                    child.gameObject.GetComponent<Renderer>().material = material;

                    Debug.Log(string.Format("Evidenzio {0}", child.name));
                }
            }
        }

        private static void AllowPartGrasping(GameObject targetObject, GameObject visibleSubAss)
        {
            targetObject.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            Collider.CreateBoxColliderOfPart(visibleSubAss);
            Collider.SetInteractionBehaviorForGrasping(targetObject, ref visibleSubAss);
            Collider.SetRigidBobyForGrasping(visibleSubAss);
        }

        private static void AllowComponentGrasping(GameObject targetObject, GameObject visibleSubAss)
        {
            targetObject.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            Collider.CreateBoxColliderOfComponent(visibleSubAss);
            Collider.SetInteractionBehaviorForGrasping(targetObject, ref visibleSubAss);
            Collider.SetRigidBobyForGrasping(visibleSubAss);
        }

        public void KatiaStopObject(GameObject gameObject)
        {

            print("Stopped grasp");

            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
