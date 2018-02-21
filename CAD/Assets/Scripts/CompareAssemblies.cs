using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CAD.Managers;
using CAD.Utility;

using System.Linq;

namespace CAD.Actions {

    public class CompareAssemblies : MonoBehaviour {

        public static CompareAssemblies instance;

        // Access to the parsed Caracteristics
        ReferencialDisplay referencialDisplay;

        // For now it is always flange-coupling-15
        GameObject queryAssembly;

        string queryLabel;

        // The other assembly we will compare to
        GameObject otherAssembly;

        string otherLabel;

        // Use this for initialization
        void Start() {

            CompareAssemblies.instance = this;

            referencialDisplay = GameObject.Find("Assemblies").GetComponent<ReferencialDisplay>();

            queryAssembly = GameObject.Find("flange-coupling-15");
            
        }

        // Update is called once per frame
        void Update() {

        }

        public void ParseLabels() {

            // Label Example
            // "nut-4, NUT311-1|flange2-1, FLANGEHUB111-1|flange-1, FLANGEHUB11-1|nut-2, NUT211-1|bolt4-4, BOLT311-1|bolt4-2, BOLT211-1|shaft2-1, SOLID111-1|shaft-1, SOLID11-1
            //|key-2, KEY11-1|key-1, KEY111-1|nut-1, NUT11-1|nut-3, NUT111-1|bolt4-1, BOLT11-1|bolt4-3, BOLT111-1|"

            // Split query label
            // Get the label from the highest local measure [0]
            queryLabel = referencialDisplay.caracteristicsList.Value[0].labels;

            // Split the '|' first
            string[] verticalBarSplit = queryLabel.Split('|');

            // Split on ','
            foreach(string correspondence in verticalBarSplit) {

                // Both parts are here now
                string[] parts = correspondence.Split(',');

                foreach(string part in parts) {

                    part.Trim();
                    print(part);
                }
            }
        }
    }
}