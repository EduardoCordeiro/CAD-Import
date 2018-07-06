using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CAD.Support;
using Leap.Unity.Interaction;
using UnityEngine;

namespace Assets.Scripts.Support
{
    class Collider
    {
        public static void CreateBoxColliderOfComponent(GameObject visibleSubAss)
        {
            BoxCollider boxCollider = visibleSubAss.gameObject.GetComponent<BoxCollider>();
            if(boxCollider == null)
                boxCollider = visibleSubAss.gameObject.AddComponent<BoxCollider>();

            Vector3 maxPoint;
            Vector3 minPoint;
            Vector3 centerAveragePoint;
            ComputeBoundingBox(visibleSubAss, out maxPoint, out minPoint, out centerAveragePoint);

            // Workaround, this is not pretty, but the collider is aligned for almost all objects
            //boxCollider.center = new Vector3(0.1f, 0.1f, -0.1f);
            boxCollider.center = centerAveragePoint;
            boxCollider.size = maxPoint - minPoint;
        }

        public static void ComputeBoundingBox(GameObject visibleSubAss, out Vector3 maxPoint, out Vector3 minPoint,
            out Vector3 centerAveragePoint)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bounds.center = Vector3.zero;

            maxPoint = Vector3.negativeInfinity;
            minPoint = Vector3.positiveInfinity;
            List<Vector3> centersList = new List<Vector3>();

            var childList = new List<Transform>();

            foreach (Transform firstLevel in visibleSubAss.transform)
            {
                childList.Add(firstLevel);
            }
            while (childList.Any())
            {
                var currentTransform = childList.First();
                childList.RemoveAt(0);

                if (currentTransform.childCount == 0)
                {
                    System.Tuple<Vector3, Vector3, Vector3> boxSize = HierarchyCreator.CreateBoxCollider(currentTransform,
                        bounds);

                    if (boxSize != null)
                    {
                        maxPoint = Vector3.Max(boxSize.Item1, maxPoint);
                        minPoint = Vector3.Min(boxSize.Item2, minPoint);
                        centersList.Add(boxSize.Item3);
                    }
                }
                else
                {
                    foreach (Transform child in currentTransform)
                    {
                        childList.Add(child);
                    }
                }
            }

            centerAveragePoint = new Vector3(centersList.Average(x => x.x), centersList.Average(x => x.y),
                centersList.Average(x => x.z));
        }

        public static void SetRigidBobyForGrasping(GameObject visibleSubAss)
        {
            var rigidBody = visibleSubAss.GetComponent<Rigidbody>();
            if (rigidBody == null)
                rigidBody = visibleSubAss.AddComponent<Rigidbody>();

            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        public static void SetInteractionBehaviorForGrasping(GameObject targetObject, ref GameObject visibleSubAss)
        {
            var interactionBehaviour = visibleSubAss.GetComponent<InteractionBehaviour>();
            if(interactionBehaviour == null)
                interactionBehaviour = visibleSubAss.AddComponent<InteractionBehaviour>();
            interactionBehaviour.enabled = true;
            interactionBehaviour.ignoreContact = true;
            interactionBehaviour.moveObjectWhenGrasped = true;
            interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Inherit;
            interactionBehaviour.graspHoldWarpingEnabled__curIgnored = false;

            //interactionBehaviour.OnGraspEnd =
            //    () => { GameObject.Find(targetObject.name).GetComponent<GestureInteraction>().StopGrasp(targetObject); };

            interactionBehaviour.ignoreGrasping = false;
        }

        public static void CreateBoxColliderOfPart(GameObject visibleSubAss)
        {
            var boxCollider = visibleSubAss.GetComponent<BoxCollider>();
            if(boxCollider == null)
                boxCollider = visibleSubAss.AddComponent<BoxCollider>();

            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bounds.center = Vector3.zero;

            if (bounds.extents == Vector3.zero)
                bounds = visibleSubAss.GetComponent<MeshFilter>().mesh.bounds;

            bounds.Encapsulate(visibleSubAss.GetComponent<MeshFilter>().mesh.bounds);

            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;
        }
    }
}
