using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CAD.Actions;

namespace CAD.Managers {

    public class ActionManager : MonoBehaviour {

        #region Statics
        /// <summary>
        /// ActionManagers singletion reference 
        /// </summary>
        public static ActionManager instance { get; private set; }
        #endregion

        List<Action> actionList;

        // Use this for initialization
        void SceneCreate() {

            ActionManager.instance = this;

            actionList = new List<Action>();
        }

        // Update is called once per frame
        void SceneUpdate() {

        }
    }
}