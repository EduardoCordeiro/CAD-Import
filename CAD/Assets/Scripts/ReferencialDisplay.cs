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

        Dictionary<string, GameObject> placeholders;

        string directoryPath = "Assets/Resources/Caracteristic Files/";

        //public float scaleFactor; // 1 for testing

        public int numberOfFiles; // 2 for testing

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            placeholders = new Dictionary<string, GameObject>();

            CollectPlaceholderData(numberOfFiles);
        }

        void CollectPlaceholderData(int numberOfFiles) {

            for(int i = 0; i < numberOfFiles; i++) {

                string fileName = i.ToString();

                StreamReader reader = new StreamReader(directoryPath + fileName + ".txt");

                string[] values = reader.ReadLine().Split('-');

                PopulateScene(sphere, CalculatePosition(values));
            }            
        }

        void PopulateScene(GameObject gameObject, Vector3 position) {

            Instantiate(gameObject, position, Quaternion.identity);
        }

        Vector3 CalculatePosition(string[] values) {

            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

        // Update is called once per frame
        void Update() {
            
        }

        void OnDrawGizmos() {

            Gizmos.color = Color.green;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0.0f, 10.0f, 0.0f));

            Gizmos.color = Color.red;

            Gizmos.DrawLine(Vector3.zero, new Vector3(10.0f, 0.0f, 0.0f));

            Gizmos.color = Color.blue;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0.0f, 0.0f, 10.0f));
        }
    }
}