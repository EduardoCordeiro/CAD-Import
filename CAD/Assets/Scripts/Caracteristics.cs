using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace CAD.Utility {

    [Serializable]
    public class Caracteristic : MonoBehaviour {

        public float shape { get; set; }

        public float position { get; set; }

        public float joint { get; set; }
    }
}