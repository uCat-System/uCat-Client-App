using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MText
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Modular 3D Text/Slider (M3D)")]
    public class MText_UI_Slider : MonoBehaviour
    {
        /// <summary>
        /// Selects on Awake()
        /// <para>Selected items can be controlled by keyboard</para>
        /// <para>If it's in a list, this is controlled by list</para>
        /// </summary>
        public bool autoFocusOnGameStart = true;

        /// <summary>
        /// As the name suggests, if it isn't interactable nothing can interact with it and gets ignored in a list
        /// </summary>
        public bool interactable = true;

        public float minValue = 0;
        public float maxValue = 100;
        [SerializeField]
        private float currentValue = 50;
        public float Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                UpdateValue();
            }
        }
        public float CurrentPercentage() => (Value / maxValue) * 100;


        public MText_UI_SliderHandle handle = null;
        public Transform progressBar = null;
        public GameObject progressBarPrefab = null;
        public Transform background = null;
        public float backgroundSize = 10;

        /// <summary>
        /// 0 is left to right
        /// </summary>
        public int directionChoice;

        [Tooltip("How much to change on key press")]
        public float keyStep = 1500;

        public bool useEvents = true;
        public UnityEvent onValueChanged = null;
        [Tooltip("Mouse/touch dragging the slider ended")]
        public UnityEvent sliderDragEnded = null;

        //visual
        public Renderer handleGraphic = null;
        public Material selectedHandleMat = null;
        public Material unSelectedHandleMat = null;
        public Material clickedHandleMat = null;
        public Material disabledHandleMat = null;

        public bool useValueRangeEvents = true;
        public bool usePercentage = true;
        public List<ValueRange> valueRangeEvents = new List<ValueRange>();
        [HideInInspector] [SerializeField] int lastValue = 0;

        [System.Serializable]
        public class ValueRange
        {
            public float min = 0;
            public float max = 25;
            public GameObject icon;
            public bool triggeredAlready;
            public UnityEvent oneTimeEvents;
            public UnityEvent repeatEvents;
        }


        #region remember inspector layout
#if UNITY_EDITOR
        [HideInInspector] public bool showKeyboardSettings = false;
        [HideInInspector] public bool showVisualSettings = false;
        [HideInInspector] public bool showEventsSettings = false;
        [HideInInspector] public bool showValueRangeSettings = false;
#endif
        #endregion remember inspector layout


        #region Unity Things
        void Awake()
        {
            if (interactable && autoFocusOnGameStart && !MText_Utilities.GetParentList(transform))
            {
                Focus(true);
            }
            else
            {
                DisabledVisual();
                this.enabled = false;
            }
        }
        #endregion Unity Things



        public void UpdateValue()
        {
            ValueChanged();
            UpdateGraphic();
        }

        /// <summary>
        /// Updates the value of the slider
        /// </summary>
        /// <param name="newValue">The parameter is the new value of the slider</param>
        public void UpdateValue(int newValue)
        {
            Value = newValue;
        }

        /// <summary>
        /// Updates the value of the slider
        /// </summary>
        /// <param name="newValue">The parameter is the new value of the slider</param>
        public void UpdateValue(float newValue)
        {
            Value = newValue;
        }

        /// <summary>
        /// Increases the value of the slider
        /// </summary>
        public void IncreaseValue()
        {
            float newValue = Value + (Time.deltaTime * keyStep);
            if (newValue > maxValue)
                newValue = maxValue;

            Value = newValue;
        }

        /// <summary>
        /// Increases the value of the slider by the given amount
        /// </summary>
        public void IncreaseValue(int amount)
        {
            float newValue = Value + amount * Time.deltaTime;

            if (newValue > maxValue)
                newValue = maxValue;

            Value = newValue;
        }
        /// <summary>
        /// Increases the value of the slider by the given amount
        /// </summary>
        public void IncreaseValue(float amount)
        {
            float newValue = Value + amount * Time.deltaTime;

            if (newValue > maxValue)
                newValue = maxValue;

            Value = newValue;
        }


        public void DecreaseValue()
        {
            float newValue =Value - (Time.deltaTime * keyStep);
            if (newValue < minValue)
                newValue = minValue;

            Value = newValue;
        }
        public void DecreaseValue(int amount)
        {
            float newValue = Value - amount * Time.deltaTime;
            if (newValue < minValue)
                newValue = minValue;

            Value = newValue;
        }
        public void DecreaseValue(float amount)
        {
            float newValue = Value - amount * Time.deltaTime;
            if (newValue < minValue)
                newValue = minValue;

            Value = newValue;
        }
        /// <summary>
        /// Selects/deselects slider
        /// </summary>
        /// <param name="enable"></param>
        public void Focus(bool enable)
        {
            this.enabled = enable;
#if ENABLE_INPUT_SYSTEM
            if (GetComponent<PlayerInput>())
            {
                if (enable)
                    GetComponent<PlayerInput>().ActivateInput();
            }
#endif

            if (enable)
                SelectedVisual();
            else
                UnSelectedVisual();
        }

        public void SelectedVisual()
        {
            var applySelectedStyle = ApplySelectedStyleFromParent();

            if (applySelectedStyle.Item1)
                ApplyVisual(applySelectedStyle.Item2.SelectedBackgroundMaterial);
            else
                ApplyVisual(selectedHandleMat);
        }
        public void UnSelectedVisual()
        {
            var applyUnselectedStyle = ApplyNormalStyleFromParent();

            if (applyUnselectedStyle.Item1)
                ApplyVisual(applyUnselectedStyle.Item2.NormalBackgroundMaterial);
            else
                ApplyVisual(unSelectedHandleMat);
        }
        public void ClickedVisual()
        {
            var applyPressedStyle = ApplyPressedStyleFromParent();

            if (applyPressedStyle.Item1)
                ApplyVisual(applyPressedStyle.Item2.PressedBackgroundMaterial);
            else
                ApplyVisual(clickedHandleMat);
        }
        public void DisabledVisual()
        {
            var applyDisabledStyle = ApplyDisabledStyleFromParent();

            if (applyDisabledStyle.Item1)
                ApplyVisual(applyDisabledStyle.Item2.DisabledBackgroundMaterial);
            else
                ApplyVisual(disabledHandleMat);
        }

        void ApplyVisual(Material handleMaterial)
        {
            if (handleGraphic)
                handleGraphic.material = handleMaterial;
        }

        public MText_UI_List GetParentList() => MText_Utilities.GetParentList(transform);

        public (bool, MText_UI_List) ApplyNormalStyleFromParent()
        {
            MText_UI_List list = GetParentList();
            if (list)
            {
                if (list.UseStyle && list.UseNormalItemVisual)
                {
                    return (true, list);
                }
            }
            //don't apply from list
            return (false, list);
        }
        public (bool, MText_UI_List) ApplySelectedStyleFromParent()
        {
            //get style from parent list
            MText_UI_List list = GetParentList();
            if (list)
            {
                if (list.UseStyle && list.UseSelectedItemVisual)
                {
                    return (true, list);
                }
            }
            //don't apply from list
            return (false, list);
        }
        public (bool, MText_UI_List) ApplyPressedStyleFromParent()
        {
            //get style from parent list
            MText_UI_List list = GetParentList();
            if (list)
            {
                if (list.UseStyle && list.UsePressedItemVisual)
                {
                    return (true, list);
                }
            }
            //don't apply from list
            return (false, list);
        }
        public (bool, MText_UI_List) ApplyDisabledStyleFromParent()
        {
            MText_UI_List list = GetParentList();

            if (list)
            {
                if (list.UseStyle && list.UseDisabledItemVisual)
                    return (true, list);
            }
            return (false, list);
        }

        /// <summary>
        /// Calls events after value is changed
        /// </summary>
        public void ValueChanged()
        {
            if (useEvents)
                onValueChanged.Invoke();
            if (useValueRangeEvents)
                ValueRangeEvents();
        }
        private void ValueRangeEvents()
        {
            //two lines can be rewritten as one
            float valueToCheckAgainst = Value;
            if (usePercentage) valueToCheckAgainst = CurrentPercentage();

            bool newRange = false;
            int newValue = 0;
            for (int i = 0; i < valueRangeEvents.Count; i++)
            {
                //correct range
                if (valueToCheckAgainst >= valueRangeEvents[i].min && valueToCheckAgainst <= valueRangeEvents[i].max)
                {
                    newValue = i;
                    if (lastValue != i) newRange = true;

                    break;
                }
            }
            if (newRange && valueRangeEvents.Count > lastValue)
            {
                if (valueRangeEvents[lastValue].icon) valueRangeEvents[lastValue].icon.SetActive(false);
                lastValue = newValue;
            }
            ProcessSelectedValueRange(newValue, newRange);
        }
        private void ProcessSelectedValueRange(int i, bool firstTime)
        {
            if (valueRangeEvents.Count <= i)
                return;

            if (valueRangeEvents[i].icon) valueRangeEvents[i].icon.SetActive(true);

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (firstTime) valueRangeEvents[i].oneTimeEvents.Invoke();
                valueRangeEvents[i].repeatEvents.Invoke();
            }
#else
            if (firstTime) valueRangeEvents[i].oneTimeEvents.Invoke();
            valueRangeEvents[i].repeatEvents.Invoke();
#endif
        }


        /// <summary>
        /// Used by raycast selector to call events after dragging handle ended
        /// </summary>
        public void ValueChangeEnded()
        {
            if (useEvents)
                sliderDragEnded.Invoke();
        }


        /// <summary>
        /// Sets slider to uninteractable
        /// </summary>
        public void Uninteractable()
        {
            interactable = false;
            DisabledVisual();
        }
        /// <summary>
        /// Sets slider to interactable
        /// </summary>
        public void Interactable()
        {
            interactable = true;
            UnSelectedVisual();
        }

        /// <summary>
        /// Creates new value range event
        /// </summary>
        public void NewValueRange()
        {
            ValueRange valueRange = new ValueRange();
            valueRangeEvents.Add(valueRange);
        }

        /// <summary>
        /// Updates the value of the slider according to position of the handle
        /// Used by raycast selector to update the value after dragging handle
        /// </summary>
        public void GetCurrentValueFromHandle()
        {
            Value = RangeConvertedValue(handle.transform.localPosition.x, (-backgroundSize / 2), (backgroundSize / 2), minValue, maxValue);
            UpdateProgressBar();
        }



        /// <summary>
        /// Updates the graphic of slider to match the value
        /// </summary>
        public void UpdateGraphic()
        {
            UpdateHandle();
            UpdateProgressBar();
        }

        private void UpdateHandle()
        {
            if (handle)
            {
                int multiplier = -1;
                if (directionChoice == 1) multiplier = 1;

                Vector3 pos = handle.transform.localPosition;
                pos.x = multiplier * RangeConvertedValue(Value, minValue, maxValue, backgroundSize / 2, -backgroundSize / 2);
                handle.transform.localPosition = pos;
            }
        }
        private void UpdateProgressBar()
        {
            if (!progressBar)
                return;

            Vector3 scale = progressBar.localScale;
            scale.x = ((Value - minValue) / (maxValue - minValue)) * backgroundSize;
            progressBar.localScale = scale;

            Vector3 pos = progressBar.localPosition;
            pos.x = -backgroundSize / 2;
            progressBar.localPosition = pos;
        }

        private float RangeConvertedValue(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
        }

        public void UpdateBackgroundSize()
        {
            background.localScale = new Vector3(backgroundSize, background.localScale.y, background.localScale.z);
        }
    }
}