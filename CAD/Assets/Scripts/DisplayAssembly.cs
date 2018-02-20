using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAssembly : MonoBehaviour {

    private List<GameObject> assembliesList;

    // Use this for initialization
    void Start () {

        assembliesList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

        print(assembliesList.Count);
	}

    public void StoreAssemblies(List<GameObject> assemblies) {

        this.assembliesList = assemblies;

        print("inside" + assemblies.Count + "   " + assembliesList.Count);
    }

    public void DisplayAssemblies() {


    }
}
