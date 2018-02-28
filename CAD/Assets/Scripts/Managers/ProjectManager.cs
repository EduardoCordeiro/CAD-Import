using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAD.Managers {

    public class ProjectManager : MonoBehaviour {

        #region Statics
        /// <summary>
        /// Project Manager instance
        /// </summary>
        public static ProjectManager instance { get; private set; }
        #endregion

        #region Managers
        /// <summary>
        /// Input Manager
        /// </summary>
        private InputManager inputManager { get; set; }

        /// <summary>
        /// Input Manager Identifier
        /// </summary>
        private string inputManagerIdentifier = "Input Manager";

        /// <summary>
        /// Input Manager
        /// </summary>
        private ActionManager actionManager { get; set; }

        /// <summary>
        /// Input Manager Identifier
        /// </summary>
        private string actionManagerIdentifier = "Action Manager";
        #endregion


        // Use this for initialization
        public void Start() {

            // Initialize the singleton instance
            ProjectManager.instance = this;

            Debug.Log("Project Manager - SceneCreate()");

            // Initilize the Input Manager
            inputManager = this.transform.Find(inputManagerIdentifier).GetComponent<InputManager>();
            inputManager.SceneCreate();

            Debug.Log("Input Manager - SceneCreate()");
        }


        // Update is called once per frame
        void Update() {

            // Keyboard and Mouse Update
            inputManager.SceneUpdate();
        }
    }
}
