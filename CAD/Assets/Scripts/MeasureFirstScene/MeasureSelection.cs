<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeasureSelection : MonoBehaviour {


    private void OnTriggerEnter(Collider collider)
    {
        print("Enter in the trigger");

        var sphereSelectedName = this.gameObject.name;

        switch (sphereSelectedName)
        {
            case "LocalSphere":
                MeasureInformation.measureType = MeasureInformation.MeasureType.Local;
                break;
            case "PartialSphere":
                MeasureInformation.measureType = MeasureInformation.MeasureType.Partial;
                break;
            case "GlobalSphere":
                MeasureInformation.measureType = MeasureInformation.MeasureType.Global;
                break;
        }

    }

    private void OnTriggerStay(Collider collider)
    {

    }


    private void OnTriggerExit(Collider collider)
    {
        SceneManager.LoadScene("Referencial");
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeasureSelection : MonoBehaviour {


    private void OnTriggerEnter(Collider collider)
    {
        print("Enter in the trigger");

        var sphereSelectedName = this.gameObject.name;

        switch (sphereSelectedName)
        {
            case "LocalSphere":
                MeasureInformation.measureType = MeasureInformation.MeasureType.Local;
                break;
            case "PartialSphere":
                MeasureInformation.measureType = MeasureInformation.MeasureType.Partial;
                break;
            case "GlobalSphere":
                MeasureInformation.measureType = MeasureInformation.MeasureType.Global;
                break;
        }

    }

    private void OnTriggerStay(Collider collider)
    {

    }


    private void OnTriggerExit(Collider collider)
    {
        SceneManager.LoadScene("Referencial");
    }
}
>>>>>>> 16b4b237dad37d5af7d6d5976c001affb80ba436
