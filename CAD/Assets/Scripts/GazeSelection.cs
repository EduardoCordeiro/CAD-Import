using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeSelection : MonoBehaviour {

    /// <summary>
    /// HitInfo property gives access to information at the object being gazed at, if any.
    /// </summary>
    public RaycastHit currentHit { get; private set; }

    public RaycastHit oldHit { get; private set; }

    /// <summary>
    /// Draw the Gaze ray
    /// </summary>
    public bool DebugDrawRay;

    public GameObject selection;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        Gaze();

        if(DebugDrawRay)
            Debug.DrawRay(this.transform.position, this.transform.forward, Color.cyan);        
    }

    public void Gaze() {

        RaycastHit hitInfo;

        // If the Raycast has succeeded and hit a sphere
        // hitInfo's point represents the position being gazed at
        // hitInfo's collider GameObject represents the assembly being gazed at
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f, Physics.DefaultRaycastLayers)) {

            currentHit = hitInfo;

            print("I hit something!" + currentHit.collider.name);

            // Timer > 2 seconds, select Object
            // here we will wait for 1-2 seconds and then select the object
            //StartCoroutine(GazeConfirmation());
            StartCoroutine(GazeConfirmation());

            // Old Hit Information
            oldHit = hitInfo;
        }
    }

    public void SelectSphere() {

        if(currentHit.collider == oldHit.collider) {

            selection = currentHit.collider.gameObject;

            selection.GetComponent<DisplayAssembly>().DisplayAssemblies();

            print("selection is correct?");
        } else
            print("Collider has changed");
    }

    IEnumerator GazeConfirmation() {
        
        yield return new WaitForSeconds(2);

        SelectSphere();
    }
}
