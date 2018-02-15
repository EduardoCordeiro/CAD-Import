using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAD.Utility {

    public static class Utility {

        public static string FixJson(string json) {

            return json = "{\"Items\":" + json + "}";
        }
    }
}