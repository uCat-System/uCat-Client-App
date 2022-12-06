using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Autohand.Demo
{
    public class OpenXRHandControllerLink : HandControllerLink {
        public InputActionProperty grabAxis;
        public InputActionProperty grabAction;
        public InputActionProperty releaseAction;
        public InputActionProperty squeezeAction;
        public InputActionProperty stopSqueezeAction;


        private bool squeezing;
        private bool grabbing;
        private void Start() {
            if(hand.left)
                handLeft = this;
            else
                handRight = this;
        }


        public void OnEnable(){
            if (grabAction == squeezeAction){
                Debug.LogError("AUTOHAND: You are using the same button for grab and squeeze on HAND CONTROLLER LINK, this will create conflict or errors", this);
            }

            if(grabAxis.action != null) grabAxis.action.Enable();
            if(grabAction.action != null) grabAction.action.performed += Grab;
            if (grabAction.action != null) grabAction.action.Enable();
            if (grabAction.action != null) grabAction.action.performed += Grab;
            if (releaseAction.action != null) releaseAction.action.Enable();
            if (releaseAction.action != null) releaseAction.action.performed += Release;
            if (squeezeAction.action != null) squeezeAction.action.Enable();
            if (squeezeAction.action != null) squeezeAction.action.performed += Squeeze;
            if (stopSqueezeAction.action != null) stopSqueezeAction.action.Enable();
            if (stopSqueezeAction.action != null) stopSqueezeAction.action.performed += StopSqueeze;
        }


        private void OnDisable(){

            if (grabAction.action != null) grabAction.action.performed -= Grab;
            if (releaseAction.action != null) releaseAction.action.performed -= Release;
            if (squeezeAction.action != null) squeezeAction.action.performed -= Squeeze;
            if (stopSqueezeAction.action != null) stopSqueezeAction.action.performed -= StopSqueeze;
        }


        private void Update() {

            hand.SetGrip(grabAxis.action.ReadValue<float>());
        }

        private void Grab(InputAction.CallbackContext grab){
            if (!grabbing){
                hand.Grab();
                grabbing = true;
            }
        }
        
        private void Release(InputAction.CallbackContext grab){
            if (grabbing){
                hand.Release();
                grabbing = false;
            }
        }

        private void Squeeze(InputAction.CallbackContext grab){
            if (!squeezing){
                hand.Squeeze();
                squeezing = true;
            }
        }
        
        private void StopSqueeze(InputAction.CallbackContext grab){
            if (squeezing){
                hand.Unsqueeze();
                squeezing = false;
            }
        }



    }
}
