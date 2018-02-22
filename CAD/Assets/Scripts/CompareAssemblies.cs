using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CAD.Managers;
using CAD.Utility;

using System.Linq;

namespace CAD.Actions {

    public class CompareAssemblies : MonoBehaviour {

        public static CompareAssemblies instance;

        // For now it is always flange-coupling-15
        public GameObject queryAssembly;

        string queryLabel;

        // The other assembly we will compare to
        GameObject otherAssembly;

        string otherLabel;

        // Use this for initialization
        void Start() {

            CompareAssemblies.instance = this;    
        }

        // Update is called once per frame
        void Update() {

        }

        public void ParseLabels(string otherObject) {

            otherAssembly = transform.Find(otherObject).gameObject;

            // Label Example
            // "nut-4, NUT311-1|flange2-1, FLANGEHUB111-1|flange-1, FLANGEHUB11-1|nut-2, NUT211-1|bolt4-4, BOLT311-1|bolt4-2, BOLT211-1|shaft2-1, SOLID111-1|shaft-1, SOLID11-1
            //|key-2, KEY11-1|key-1, KEY111-1|nut-1, NUT11-1|nut-3, NUT111-1|bolt4-1, BOLT11-1|bolt4-3, BOLT111-1|"

            // Split query label
            // Get the label from the highest local measure [0]
            queryLabel = ReferencialDisplay.instance.caracteristicsList[otherAssembly][0].labels;

            // Split the '|' first
            string[] verticalBarSplit = queryLabel.Split('|');

            int numberOfParts = verticalBarSplit.Length;
            int counter = 0;

            // Split on ','
            foreach(string correspondence in verticalBarSplit) {

                // ignoring the last white space
                if(correspondence == "")
                    continue;

                // Both parts are here now
                string[] parts = correspondence.Split(',');

                string queryPart = parts[0].Trim();
                string otherPart = parts[1].Trim();

                Color partColor = new Color(counter / numberOfParts, 1 - counter / numberOfParts, 0);

                ColorAssemblyParts(queryPart, otherPart, partColor);

                counter++;
            }
        }

        public void ColorAssemblyParts(string queryPart, string otherPart, Color color) {

            GameObject queryAssemblyPart = SearchChild(queryAssembly.transform, queryPart);

            GameObject otherAssemblyPart = SearchChild(otherAssembly.transform, otherPart);

            queryAssemblyPart.GetComponent<Renderer>().material.color = color;

            otherAssemblyPart.GetComponent<Renderer>().material.color = color;
        }

        GameObject SearchChild(Transform parent, string name) {

            GameObject missingChild = null;

            foreach(Transform child in parent) {

                if(child.name == name)
                    return child.gameObject;
                else
                    missingChild = SearchChild(child, name);
            }

            return missingChild;
        }
    }
}