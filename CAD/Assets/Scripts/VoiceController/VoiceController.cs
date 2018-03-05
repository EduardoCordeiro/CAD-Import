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
using UnityEngine.Windows.Speech;

namespace Assets.Scripts.VoiceController
{
    class VoiceController : MonoBehaviour
    {
        [SerializeField]
        private string[] m_Keywords;

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
                    GetComponent<ObjectExplosion>().CircleExplosion(0.3f, CompareAssemblies.instance.otherAssembly);
                    break;
            }
        }
    }
}
