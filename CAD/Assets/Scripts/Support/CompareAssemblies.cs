using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CAD.Managers;
using CAD.Utility;

using System.Linq;

namespace CAD.Support {

    public class CompareAssemblies : MonoBehaviour {

        public static CompareAssemblies instance;

        // For now it is always flange-coupling-15
        public GameObject queryAssembly;

        string queryLabel;

        // The other assembly we will compare to
        GameObject otherAssembly;

        string otherLabel;

        private void Awake() {

            instance = this;
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void ParseLabels(string otherObject) {

            otherAssembly = transform.Find(otherObject).gameObject;

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

                // casts are required
                Color partColor = new Color((float)((float)counter / (float)numberOfParts), ((float)1 - (float)counter / (float)numberOfParts), 0);

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

        GameObject SearchChild(Transform parent, string name)
        {
            GameObject missingChild = null;
            name.Replace("\\", string.Empty);

            var parentList = name.Split('/').ToList();

            Transform currentParent = null;

            while (parentList.Any())
            {
                foreach (Transform child in parent)
                {
                    if (child.name == parentList.First())
                    {
                        parentList.Remove(parentList.First());
                        currentParent = child;
                        break;
                    }
                }
                parent = currentParent;
            }

            missingChild = currentParent.gameObject;
            return missingChild;
        }
    }
}