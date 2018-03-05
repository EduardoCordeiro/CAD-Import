using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Text;
using CAD.Actions;
using CAD.Support;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

namespace Assets.Scripts.VoiceController
{
    class VoiceController : MonoBehaviour
    {
        [SerializeField]
        private string[] m_Keywords;

        private List<Vector3> originalParts;
        private KeywordRecognizer m_Recognizer;

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

            switch (args.text)
            {
                case "Nascondi":
                    break;
                case "Mostra":
                    break;
                case "Decomponi":
                    break;
                case "Next":
                    break;
                case "Esplodi":
                    originalParts = GetComponent<ObjectExplosion>().CircleExplosion(0.3f, CompareAssemblies.instance.otherAssembly);
                    break;
                case "Assembla":
                    GetComponent<ObjectExplosion>().ReverseExplosion(CompareAssemblies.instance.otherAssembly, originalParts);
                    break;
                case "Indietro":
                    var gazeSelcetion = GameObject.Find("CenterEyeAnchor").GetComponent<GazeSelection>();
                    switch (gazeSelcetion.phase)
                    {
                        case Phase.None:
                            SceneManager.LoadScene("MeasureSelection");
                            break;
                        case Phase.AssemblySelection:
                            Scene loadLevel = SceneManager.GetActiveScene();
                            SceneManager.LoadScene(loadLevel.name);
                            break;
                        case Phase.AssemblyComparision:
                            if (gazeSelcetion.phaseHistory[gazeSelcetion.phaseHistory.Count - 2] == Phase.AssemblySelection)
                            {
                                gazeSelcetion.ToggleAssemblies(false);

                                var spheres = ReferencialDisplay.instance.sphereRepresentationList;
                                //var lastSphereGameObject = gazeSelcetion.oldHit.collider.name;
                                var lastSphere = spheres.Find(s => s.GetComponent<DisplayAssembly>().assembliesList.Find(obj => obj.name == CompareAssemblies.instance.otherAssembly.name));
                                Debug.Log("Vengo dalla sfera: " + lastSphere.gameObject.name);

                                lastSphere.GetComponent<DisplayAssembly>().DisplayAssemblies();
                                gazeSelcetion.phase = Phase.AssemblySelection;
                            }
                            else if (gazeSelcetion.phaseHistory[gazeSelcetion.phaseHistory.Count - 2] == Phase.None)
                            {
                                Scene loadPreviousLevel = SceneManager.GetActiveScene();
                                SceneManager.LoadScene(loadPreviousLevel.name);
                            }
                            break;
                        case Phase.Done:
                            if (gazeSelcetion.phaseHistory[gazeSelcetion.phaseHistory.Count - 2] == Phase.AssemblySelection)
                            {
                                gazeSelcetion.ToggleAssemblies(false);

                                var spheres = ReferencialDisplay.instance.sphereRepresentationList;
                                //var lastSphereGameObject = gazeSelcetion.oldHit.collider.name;
                                var lastSphere = spheres.Find(s => s.GetComponent<DisplayAssembly>().assembliesList.Find(obj => obj.name == CompareAssemblies.instance.otherAssembly.name));
                                Debug.Log("Vengo dalla sfera: " + lastSphere.gameObject.name);
                                lastSphere.GetComponent<DisplayAssembly>().DisplayAssemblies();
                                gazeSelcetion.phase = Phase.AssemblySelection;
                            }
                            else if (gazeSelcetion.phaseHistory[gazeSelcetion.phaseHistory.Count - 2] == Phase.None)
                            {
                                Scene loadPreviousLevel = SceneManager.GetActiveScene();
                                SceneManager.LoadScene(loadPreviousLevel.name);
                            }
                            break;
                    }
                    break;
            }
        }
    }
}
