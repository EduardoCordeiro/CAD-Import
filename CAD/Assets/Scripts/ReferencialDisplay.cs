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

        float scaleFactor;

        // Use this for initialization
        void Start() {

            sphere = Resources.Load<GameObject>("Prefabs/Sphere");

            placeholders = new List<GameObject>();

            scaleFactor = 1.0f;

            PopulateScene();
        }

        void PopulateScene() {

            Instantiate(sphere, new Vector3(0.5f, 0.1f, 0.9f), Quaternion.identity);

            Instantiate(sphere, new Vector3(0.1f, 0.7f, 0.2f), Quaternion.identity);
        }

        Vector3 CalculatePosition(float shape, float position, float joint) {

            return new Vector3(shape, position, joint) * scaleFactor;
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