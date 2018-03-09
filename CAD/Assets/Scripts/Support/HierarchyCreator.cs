using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;

namespace CAD.Support {

    public class HierarchyCreator : MonoBehaviour { 

        // Use this for initialization
        void Awake() {

            CalculateBoudingBox();

            this.gameObject.layer = 14;
        }

        // Update is called once per frame
        void Update() {

        }

        private void CalculateBoudingBox() {
            
            BoxCollider boxCollider = this.gameObject.AddComponent<BoxCollider>();

            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bounds.center = Vector3.zero;
            
            Vector3 maxPoint = Vector3.negativeInfinity;
            Vector3 minPoint = Vector3.positiveInfinity;
            List<Vector3> centersList = new List<Vector3>();
                        
            foreach(Transform child in this.transform) {

                System.Tuple<Vector3, Vector3, Vector3> boxSize = CreateBoxCollider(child, bounds);

                if(boxSize == null)
                    continue;

                maxPoint = Vector3.Max(boxSize.Item1, maxPoint);
                minPoint = Vector3.Min(boxSize.Item2, minPoint);
                centersList.Add(boxSize.Item3);
            }

           
            var centerAveragePoint = new Vector3(centersList.Average(x => x.x), centersList.Average(x => x.y), centersList.Average(x => x.z));

            // Workaround, this is not pretty, but the collider is aligned for almost all objects
            //boxCollider.center = new Vector3(0.1f, 0.1f, -0.1f);
            boxCollider.center = centerAveragePoint;
            boxCollider.size = maxPoint - minPoint;
        }

        private System.Tuple<Vector3, Vector3, Vector3> CreateBoxCollider(Transform meshTransform, Bounds bounds) {

            MeshFilter meshFilter = meshTransform.GetComponent<MeshFilter>();            

            if(meshFilter == null)
            {
                //Find new childs
                foreach(Transform child in meshTransform)
                     return CreateBoxCollider(child, bounds);

                return null;
            }
            else
            {

                if (bounds.extents == Vector3.zero)
                    bounds = meshFilter.mesh.bounds;
                bounds.Encapsulate(meshFilter.mesh.bounds);

                return new System.Tuple<Vector3, Vector3, Vector3>(meshFilter.mesh.bounds.max, meshFilter.mesh.bounds.min, meshFilter.mesh.bounds.center);
            }
        }

        public void CreateHierarchy() {

            MakeChild();
            
            ClearDuplicateGameObjects();

            Hierarchy();
        }

        /// <summary>
        /// Make all parts be children of the root object
        /// </summary>
        private void MakeChild() {

            List<GameObject> rootObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);

            foreach(GameObject go in rootObjects)
                if(go.GetComponent<MeshRenderer>() != null)
                    go.transform.parent = this.transform;
        }

        /// <summary>
        /// Clear the duplicate that comes from the import software
        /// </summary>
        private void ClearDuplicateGameObjects() {

            for(int i = 0; i < transform.childCount; i++)
                for(int j = i + 1; j < transform.childCount; j++)
                    if(transform.GetChild(i).name == transform.GetChild(j).name)
                        DestroyImmediate(transform.GetChild(j).gameObject);
        }

        /// <summary>
        /// Create the object hierarchy
        /// </summary>
        private void Hierarchy() {

            List<Transform> children = new List<Transform>();

            foreach(Transform child in this.transform)
                children.Add(child);

            foreach(Transform c in children) {

                List<string> name = c.name.Split(' ').ToList();

                GameObject currentObject = c.gameObject;

                c.name = name.Last();

                name.Remove(name.Last());

                while(name.Count > 0) { 

                    GameObject parent = GameObject.Find(name.Last());
                    
                    if(parent == null) {

                        parent = new GameObject(name.Last());
                        parent.transform.parent = this.transform;
                    }

                    currentObject.transform.parent = parent.transform;

                    currentObject = parent;

                    name.Remove(name.Last());
                }
            }
        }
    }
}