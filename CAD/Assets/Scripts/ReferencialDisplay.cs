using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System;

namespace CAD.Managers {

    enum Caracteristic {

        Shape, // X
        Position, // Y
        Joint // Z
    }

    public class ReferencialDisplay : MonoBehaviour {

        GameObject sphere;

        List<GameObject> placeholders;

        string directoryPath = "Assets/Resources/Caracteristic Files/";

        //public float scaleFactor; // 1 for testing

        public int numberOfFiles; // 2 for testing

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            placeholders = new List<GameObject>();

            numberOfFiles = this.GetComponentsInChildren<Transform>().Length;

            CollectPlaceholderData(numberOfFiles);
        }

        // Update is called once per frame
        void Update() {

        }

        void CollectPlaceholderData(int numberOfFiles) {

            for(int i = 0; i < numberOfFiles; i++) {

                /*string fileName = i.ToString();

                StreamReader reader = new StreamReader(directoryPath + fileName + ".txt");

                string[] values = reader.ReadLine().Split('-');

                PopulateScene(sphere, CalculatePosition(values));*/

                Vector3 position = new Vector3(UnityEngine.Random.Range(0.0f, 2.0f), UnityEngine.Random.Range(1.5f, 3.5f), UnityEngine.Random.Range(0.0f, 2.0f));

                PopulateScene(sphere, position);
            }

            // Add some fake spheres to hightlit the referencial
            PopulateScene(sphere, new Vector3(5.0f, 0.0f, 0.0f));
            ColorGameObject(Color.red, placeholders[placeholders.Count - 1]);

            PopulateScene(sphere, new Vector3(0.0f, 5.0f, 0.0f));
            ColorGameObject(Color.green, placeholders[placeholders.Count - 1]);

            PopulateScene(sphere, new Vector3(0.0f, 0.0f, 5.0f));
            ColorGameObject(Color.blue, placeholders[placeholders.Count - 1]);

            PopulateScene(sphere, new Vector3(5.0f, 5.0f, 5.0f));
            ColorGameObject(Color.black, placeholders[placeholders.Count - 1]);
        }

        void PopulateScene(GameObject gameObject, Vector3 position) {

            GameObject placeholder = Instantiate(gameObject, position, Quaternion.identity);

            placeholders.Add(placeholder);
        }

        void ColorGameObject(Color color, GameObject gameObject) {

            gameObject.GetComponent<Renderer>().material.color = color;
        }

        Vector3 CalculatePosition(string[] values) {

            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

        void OnDrawGizmos() {

            Gizmos.color = Color.red;

            Gizmos.DrawLine(Vector3.zero, new Vector3(10.0f, 0.0f, 0.0f));

            Gizmos.color = Color.green;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0.0f, 10.0f, 0.0f));

            Gizmos.color = Color.blue;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0.0f, 0.0f, 10.0f));
        }
    }
}