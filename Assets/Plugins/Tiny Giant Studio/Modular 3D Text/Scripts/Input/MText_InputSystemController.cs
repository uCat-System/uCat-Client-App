using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif



namespace MText
{
    [AddComponentMenu("Modular 3D Text/Input System (M3D)")]
    public class MText_InputSystemController : MonoBehaviour
    {
        public UnityEvent upAxisEvent;
        public UnityEvent downAxisEvent;
        public UnityEvent leftAxisEvent;
        public UnityEvent rightAxisEvent;
        public UnityEvent submitEvent;

#if ENABLE_INPUT_SYSTEM
        public UnityEvent tabEvent;
        [Space(10)]
        public InputAction horizontalAxis;
#elif ENABLE_LEGACY_INPUT_MANAGER
        /// <summary>
        /// How long you have to press a key down for it to register as a second key press
        /// </summary>
        [Tooltip("How long you have to press a key down for it to register as a second key press")]
        public float tickRate = 0.25f;

        public bool axisInput = true;

        private float lastPressedUp;

        private float lastPressedDown;

        private float lastPressedLeft;

        private float lastPressedRight;

        private float lastPressedSubmit;


        private float axisSensitivity = 0.1f;
        private string verticalAxisString = "Vertical";
        private string horizontalAxisString = "Horizontal";
        private string submitString = "Submit";

        [HideInInspector]
        public List<StandardInput> inputs = new List<StandardInput>();

        [System.Serializable]
        public class StandardInput
        {
            public KeyCode key;
            public UnityEvent unityEvent;
            [HideInInspector]
            public float lastPressed;
        }
#endif
#if ENABLE_INPUT_SYSTEM

#elif ENABLE_LEGACY_INPUT_MANAGER
        void Update()
        {
            if (axisInput)
                AxisInputController();


            DefaultInputController();
        }
#endif


#if ENABLE_INPUT_SYSTEM
        void OnNavigate(InputValue value)
        {
            if (!gameObject.activeInHierarchy)
                return;

            //Debug.Log(value.Get<Vector2>().y);


            if (value.Get<Vector2>().y > 0)
                upAxisEvent.Invoke();
            else if (value.Get<Vector2>().y < 0)
                downAxisEvent.Invoke();


            if (value.Get<Vector2>().x > 0)
                rightAxisEvent.Invoke();
            else if (value.Get<Vector2>().x < 0)
                leftAxisEvent.Invoke();
        }

        void OnSubmit(InputValue value)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (!value.isPressed)
                return;

            submitEvent.Invoke();
        }

        void OnTab(InputValue value)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (!value.isPressed)
                return;

            tabEvent.Invoke();
        }


#elif ENABLE_LEGACY_INPUT_MANAGER
        private void AxisInputController()
        {
            //If you are having error here, please check if you changed the string for Horizontal axis and change the value of horizontalAxisString accordingly
            if (Input.GetAxis(horizontalAxisString) < -axisSensitivity)
            {
                if (tickRate > 0)
                {
                    if (lastPressedLeft + tickRate < Time.time)
                    {
                        lastPressedLeft = Time.time;
                        leftAxisEvent.Invoke();
                    }
                }
                else
                {
                    leftAxisEvent.Invoke();
                }
            }

            //If you are having error here, please check if you changed the string for Horizontal axis and change the value of horizontalAxisString accordingly
            if (Input.GetAxis(horizontalAxisString) > axisSensitivity)
            {
                if (tickRate > 0)
                {
                    if (lastPressedRight + tickRate < Time.time)
                    {
                        lastPressedRight = Time.time;
                        rightAxisEvent.Invoke();
                    }
                }
                else
                {
                    rightAxisEvent.Invoke();
                }
            }

            //If you are having error here, please check if you changed the string for Vertical axis and change the value of verticalAxisString accordingly
            if (Input.GetAxis(verticalAxisString) > axisSensitivity)
            {
                if (tickRate > 0)
                {
                    if (lastPressedUp + tickRate < Time.time)
                    {
                        lastPressedUp = Time.time;
                        upAxisEvent.Invoke();
                    }
                }
                else
                {
                    upAxisEvent.Invoke();
                }
            }


            //If you are having error here, please check if you changed the string for Vertical axis and change the value of verticalAxisString accordingly
            if (Input.GetAxis(verticalAxisString) < -axisSensitivity)
            {
                if (tickRate > 0)
                {
                    if (lastPressedDown + tickRate < Time.time)
                    {
                        lastPressedDown = Time.time;
                        downAxisEvent.Invoke();
                    }
                }
                else
                {
                    upAxisEvent.Invoke();
                }
            }

            if (Input.GetButton(submitString))
            {
                if (tickRate > 0)
                {
                    if (lastPressedSubmit + tickRate < Time.time)
                    {
                        lastPressedSubmit = Time.time;
                        submitEvent.Invoke();
                    }
                }
                else
                {
                    submitEvent.Invoke();
                }
            }
        }

        private void DefaultInputController()
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                if (KeyPressed(i))
                {
                    if (inputs[i].lastPressed + tickRate < Time.time)
                    {
                        inputs[i].lastPressed = Time.time;
                        inputs[i].unityEvent.Invoke();
                    }
                }
            }
        }

        private bool KeyPressed(int i)
        {
            if (inputs[i].lastPressed + tickRate > Time.time)
                return false;

            return Input.GetKey(inputs[i].key);
        }       
#endif
    }
}
