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

    public void DisplayAssemblies() {

        print("I will display these assemblies");
        foreach(GameObject assembly in assembliesList)
            print(assembly.name);
    }
}
