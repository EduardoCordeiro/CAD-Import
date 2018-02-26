using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAssembly : MonoBehaviour {

    private List<GameObject> assembliesList;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    public void StoreAssemblies(List<GameObject> assemblies) {

        this.assembliesList = new List<GameObject>(assemblies);
    }

    public int DisplayAssemblies(Vector3 hitPosition) {

        print("Number of assemblies to display: " + assembliesList.Count);

        Vector3 offset = new Vector3(0.4f, 0.0f, 0.0f);

        foreach(GameObject assembly in assembliesList) {

            assembly.SetActive(true);

            assembly.transform.position = hitPosition + offset;

            hitPosition += offset;
        }

        return assembliesList.Count;
    }
}
