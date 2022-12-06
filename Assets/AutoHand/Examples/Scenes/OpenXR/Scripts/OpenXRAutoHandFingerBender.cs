using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Autohand.Demo{
    public class OpenXRAutoHandFingerBender : MonoBehaviour{
        public Hand hand;
        public InputActionProperty bendAction;
        public InputActionProperty unbendAction;
        
        [HideInInspector]
        public float[] bendOffsets;

        bool pressed;

        private void OnEnable() {
            if (bendAction.action != null) bendAction.action.Enable();
            if (bendAction.action != null) bendAction.action.performed += BendAction;
            if (unbendAction.action != null) unbendAction.action.Enable();
            if (unbendAction.action != null) unbendAction.action.performed += UnbendAction;
        }
        private void OnDisable(){
            if (bendAction.action != null) bendAction.action.performed -= BendAction;
            if (unbendAction.action != null) unbendAction.action.performed -= UnbendAction;
        }

        void BendAction(InputAction.CallbackContext a) {
            if (!pressed) {
                pressed = true;
                for(int i = 0; i < hand.fingers.Length; i++) {
                    hand.fingers[i].bendOffset += bendOffsets[i];
                }
            }
        }

        void UnbendAction(InputAction.CallbackContext a) {
            if (pressed) {
                pressed = false;
                for(int i = 0; i < hand.fingers.Length; i++) {
                    hand.fingers[i].bendOffset -= bendOffsets[i];
                }
            }
        }
    }
}
