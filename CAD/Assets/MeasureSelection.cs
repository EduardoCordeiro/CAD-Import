using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureSelection : MonoBehaviour {

	public enum measureType
	{
	    Global = 0,
        Partial = 1,
        Local = 2
	}

    public measureType selectedMeasure;

    private void OnAwake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collider)
    {
        print("Enter in the trigger");

        var sphereSelectedName = this.gameObject.name;

        switch (sphereSelectedName)
        {
            case "LocalSphere":
                selectedMeasure = measureType.Local;
                break;
            case "PartialSphere":
                selectedMeasure = measureType.Partial;
                break;
            case "GlobalSphere":
                selectedMeasure = measureType.Global;
                break;
        }

    }

    private void OnTriggerStay(Collider collider)
    {

    }


    private void OnTriggerExit(Collider collider)
    {
        print("Exit from the trigger " + selectedMeasure.ToString());
    }
}
