﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;

public class GestureInteraction : MonoBehaviour {

    Controller controller;

	// Use this for initialization
	void Start () {

        controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {

        Frame frame = controller.Frame();

        
	}
}