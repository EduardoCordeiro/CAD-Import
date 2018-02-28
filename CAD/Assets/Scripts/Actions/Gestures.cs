using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInteraction : MonoBehaviour {

    int counter = 0;

    public void StartPrint() {


        //while(true) {

            print("My confidence is increasing :: " + counter++);
        //}
    }

    public void StopPrint() {

        //while(true) {

            print("My confidence is decreasing :: " + counter--);
        //}
    }
}
