using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAD.Managers {

    public class InputManager : MonoBehaviour {

        #region Statics
        /// <summary>
        /// ActionManagers singletion reference 
        /// </summary>
        public static InputManager instance { get; private set; }
        #endregion

        #region Attributes
        /// <summary>
        /// If using the keyboard or (Gestures or Controllers)
        /// </summary>
        public bool usingKeyboard = false;
        #endregion

        /// <summary>
        /// Creates the handlers
        /// </summary>
        public void SceneCreate() {

            InputManager.instance = this;
        }

        /// <summary>
        /// Updates the handlers
        /// </summary>
        public void SceneUpdate() {

            if(usingKeyboard)
                ReadKeyboard();
        }

        /// <summary>
        /// Reads the keyboard and calls updates the action
        /// </summary>
        public void ReadKeyboard() {

            //if(Input.GetKeyDown(KeyCode.F))
              //  ActionManager.instance.Explosion();

            //if(Input.GetKeyDown(KeyCode.G))
             //   ActionManager.instance.CircleExplosion();

            //if(Input.GetKeyDown(KeyCode.R))
               // ActionManager.instance.ReverseExplosion();
        }
    }
}