using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    #region Statics
    /// <summary>
    /// Project Manager instance
    /// </summary>
    public static Manager instance { get; private set; }
    #endregion


    // Use this for initialization
    public void Start() {

        // Initialize the singleton instance
        Manager.instance = this;

        Debug.Log("Hand Manager - SceneCreate()");
    }


    // Update is called once per frame
    void Update () {
		
	}
}
