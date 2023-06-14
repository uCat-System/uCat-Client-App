using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif


namespace MText
{
    /// <summary>
    /// Handles input for raycast selector
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MText_UI_RaycastSelector))]
    public class MText_UI_RaycastSelectorInputProcessor : MonoBehaviour
    {
        #region Raycast settings
        [Tooltip("If not assigned, it will automatically get Camera.main on Start")]
        public Camera myCamera;
        #endregion Raycast settings


        Transform currentTarget = null;
        bool dragging = false;

        MText_UI_RaycastSelector raycastSelector;



        #region Unity Things
        void Awake()
        {
            raycastSelector = GetComponent<MText_UI_RaycastSelector>();
#if ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Enable();
#endif
        }

        void Start()
        {
            //If no camera assigned, get Camera.main
            if (!myCamera)
                myCamera = Camera.main;
        }

        void Update()
        {
            if (!myCamera)
                return;

            //If Already dragging stuff, do dragging stuff
            if (dragging)
            {
                raycastSelector.Dragging(currentTarget);
                DetectDragEnd();
            }
            else
            {
                //Check if mouse is on something
                Transform pointerOnUI = RaycastCheck();

                //If mouse not on the old UI, unselect old one
                if (pointerOnUI != currentTarget)
                    raycastSelector.UnSelectOldTarget(currentTarget);


                //If mouse on a UI
                if (pointerOnUI)
                {
                    //If it's a new target, select that
                    if (pointerOnUI != currentTarget)
                        raycastSelector.SelectNewTarget(pointerOnUI);

                    //If the UI is clicked 
                    if (PressedButton())
                    {
                        raycastSelector.PressTarget(pointerOnUI);
                        dragging = true;
                    }
                }

                currentTarget = pointerOnUI;
            }
        }
        #endregion Unity things




        bool PressedButton()
        {
#if ENABLE_INPUT_SYSTEM
            if (MouseClicked() || Tapped())
                return true;
            return false;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

#if ENABLE_INPUT_SYSTEM
        bool MouseClicked()
        {
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasPressedThisFrame;

            return false;
        }

        bool Tapped()
        {
            if (Touch.activeTouches.Count > 0)
                return Touch.activeTouches[0].ended;

            return false;
        }
#endif

        Transform RaycastCheck()
        {
#if ENABLE_INPUT_SYSTEM
            Ray ray = myCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
#else
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
#endif
            return raycastSelector.RaycastCheck(ray);
        }

        void DetectDragEnd()
        {
            if (MouseButtonReleased() && dragging)
            {
                dragging = false;
                raycastSelector.DragEnded(currentTarget, RaycastCheck());
            }

            if (!Input.touchSupported)
                return;

            if (Input.touchCount > 0)
            {
#if ENABLE_INPUT_SYSTEM
                if (Input.touches[0].phase == UnityEngine.TouchPhase.Ended)
#else
                if (Input.touches[0].phase == TouchPhase.Ended)
#endif
                {
                    dragging = false;
                    raycastSelector.DragEnded(currentTarget, RaycastCheck());
                }
            }
            else
            {
                dragging = false;
                raycastSelector.DragEnded(currentTarget, RaycastCheck());
            }
        }

        private bool MouseButtonReleased()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasReleasedThisFrame;
            return false;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }
    }
}
