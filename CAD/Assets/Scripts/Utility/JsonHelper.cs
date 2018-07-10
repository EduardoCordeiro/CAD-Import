using System;
using System.Linq;
using UnityEngine;

namespace CAD.Utility {

    public static class JsonHelper {

        public static T[] FromJson<T>(string json) {

            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);

            return wrapper.Items;
        }

        public static T ElementFromJson<T>(string json)
        {

            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);

            return wrapper.Items.First();
        }

        public static string ToJson<T>(T[] array) {

            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;

            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint) {

            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;

            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T> {

            public T[] Items;
        }

        public static string FixJson(string json) {

            return json = "{\"Items\":" + json + "}";
        }


        public static string FixJsonMeasure(string json)
        {
            var Start = json.IndexOf("\"Measure\":");
            var End = json.Length - 1;

            return json = "{\"Items\":[" + json.Substring(Start + 10, End - Start - 10 - 2) + "]}";
        }
    }
}