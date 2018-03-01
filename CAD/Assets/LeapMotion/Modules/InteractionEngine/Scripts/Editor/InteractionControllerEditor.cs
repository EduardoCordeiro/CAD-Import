/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using Leap.Unity.Query;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Leap.Unity.Interaction {

  [CanEditMultipleObjects]
  [CustomEditor(typeof(InteractionController), true)]
  public class InteractionControllerEditor : CustomEditorBase<InteractionController> {

    public override void OnInspectorGUI() {
      checkParentedToManager();
      checkWithinHandPoolParent();
      checkPrimaryHoverPoints();

      base.OnInspectorGUI();
    }

    private void checkPrimaryHoverPoints() {
      bool anyPrimaryHoverPoints = false;
      bool anyWithNoPrimaryHoverPoints = false;
      foreach (var singleTarget in targets) {
        anyPrimaryHoverPoints = false;
        foreach (var primaryHoverPoint in singleTarget.primaryHoverPoints) {
          if (primaryHoverPoint != null) {
            anyPrimaryHoverPoints = true;
            break;
          }
        }

        if (singleTarget.intHand != null) {
          for (int i = 0; i < singleTarget.intHand.enabledPrimaryHoverFingertips.Length; i++) {
            if (singleTarget.intHand.enabledPrimaryHoverFingertips[i]) {
              anyPrimaryHoverPoints = true;
              break;
            }
          }
        }

        if (!anyPrimaryHoverPoints) {
          anyWithNoPrimaryHoverPoints = true;
          break;
        }
      }

      if (anyWithNoPrimaryHoverPoints) {
        string message = "No primary hover points found for this interaction controller. "
                       + "This controller will never trigger primary hover for an object. "
                       + "UI elements such as InteractionButton and InteractionSlider "
                       + "will not be able to interact with this interaction controller.";
        EditorGUILayout.HelpBox(message, MessageType.Warning);
      }
    }

    private void checkParentedToManager() {
      bool plural = targets.Length > 1;
      bool anyNotParentedToInteractionManager;

      anyNotParentedToInteractionManager = targets.Query()
                                                  .Any(c => c.GetComponentInParent<InteractionManager>() == null);

      if (anyNotParentedToInteractionManager) {
        string message = "";
        if (plural) {
          message += "One of more currently selected controllers ";
        }
        else {
          message += "The currently selected controller ";
        }

        message += "is not the child of an Interaction Manager. Interaction Controllers "
                 + "must be childed to an Interaction Manager in order to function.";

        EditorGUILayout.HelpBox(message, MessageType.Warning);
      }
    }

    private void checkWithinHandPoolParent() {
      bool plural = targets.Length > 1;
      bool anyWithinHandPool;

      HandPool handPool = FindObjectOfType<HandPool>();
      if (handPool == null) return;

      anyWithinHandPool = targets.Query()
                                 .Any(c => c.transform.parent == handPool.modelsParent
                                        && handPool.modelsParent != null);

      if (anyWithinHandPool) {
        string message = "";
        if (plural) {
          message += "One or more of the currently selected controllers ";
        }
        else {
          message += "The currently selected controller ";
        }

        message += "is a within the HandPool model parent. Interaction controllers, such "
                 + "as InteractionHands, are not HandModels and are not spawned by the "
                 + "HandPool. InteractionHands and all Interaction controllers should "
                 + "be childed to the Interaction Manager.";

        EditorGUILayout.HelpBox(message, MessageType.Error);
      }
    }

  }

}
