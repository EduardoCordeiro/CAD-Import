using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using Leap;
using Leap.Unity.Interaction;

namespace CAD.Support {

    public class GestureInteraction : MonoBehaviour {

        public InteractionBehaviour interactionBehaviour;
        List<InteractionBehaviour> behavioursList;

        // Use this for initialization
        void Start() {

            behavioursList = new List<InteractionBehaviour>();

            foreach(Transform child in transform)
                behavioursList.Add(child.GetComponent<InteractionBehaviour>());
        }

        // Update is called once per frame
        void Update() {

        }

        /// <summary>
        /// Thumbs Up Gestures
        /// Move this to another class that stores the state we are in
        /// </summary>
        public void PreviousState() {

            Debug.Log("ClosedFist()");
        }

        public void OpenHand() {

            Debug.Log("OpenHand");
        }

        public void PointingFinger() {

            print("PointingFinger()");
        }

        public void NotPointing() {

            print("NotPointing()");
        }


        // Grasping
        public void GraspPart() {

            print("I began a grasp");
        }

        public void StopGrasp(GameObject gameObject) {

            print("Stopped grasp");

            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }


    }
}