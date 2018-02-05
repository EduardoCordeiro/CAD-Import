using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAD.Managers {

    public class ActionManager : MonoBehaviour {

        #region Statics
        /// <summary>
        /// ActionManagers singletion reference 
        /// </summary>
        public static ActionManager instance { get; private set; }
        #endregion

        // Use this for initialization
        void SceneCreate() {

            ActionManager.instance = this;
        }

        // Update is called once per frame
        void SceneUpdate() {

        }

        void Explosion() {

        }

        void CircleExplosion() {


        }

        void ReverseExplosion() {


        }
    }
}