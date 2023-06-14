using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace MText
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Modular 3D Text/List (M3D)")]
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/scripts/list")]
    public class MText_UI_List : MonoBehaviour
    {
        [Tooltip("List is scrollable with keyboard when focused")]
        [SerializeField] private bool autoFocusOnStart = true;
        [Tooltip("Selects first item when focused")]
        [SerializeField] private bool autoFocusFirstItem = true;



        #region Style options -------------------------------------------------------------------

        [SerializeField]
        private bool _useStyle = false;
        public bool UseStyle
        {
            get { return _useStyle; }
            set
            {
                _useStyle = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private bool _useNormalItemVisual = false;
        public bool UseNormalItemVisual
        {
            get { return _useNormalItemVisual; }
            set
            {
                _useNormalItemVisual = value;
                UpdateStyle();
            }
        }


        [SerializeField]
        private Vector3 _normalTextSize = new Vector3(10, 10, 1f);
        public Vector3 NormalTextSize
        {
            get { return _normalTextSize; }
            set
            {
                _normalTextSize = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Material _normalTextMaterial = null;
        public Material NormalTextMaterial
        {
            get { return _normalTextMaterial; }
            set
            {
                _normalTextMaterial = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Material _normalBackgroundMaterial = null;
        public Material NormalBackgroundMaterial
        {
            get { return _normalBackgroundMaterial; }
            set
            {
                _normalBackgroundMaterial = value;
                UpdateStyle();
            }
        }



        [SerializeField]
        private bool _useSelectedItemVisual = false;
        public bool UseSelectedItemVisual
        {
            get { return _useSelectedItemVisual; }
            set
            {
                _useSelectedItemVisual = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Vector3 _selectedTextSize = new Vector3(10.5f, 10.5f, 5f);
        public Vector3 SelectedTextSize
        {
            get { return _selectedTextSize; }
            set
            {
                _selectedTextSize = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Material _selectedTextMaterial = null;
        public Material SelectedTextMaterial
        {
            get { return _selectedTextMaterial; }
            set
            {
                _selectedTextMaterial = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Material _selectedBackgroundMaterial = null;
        public Material SelectedBackgroundMaterial
        {
            get { return _selectedBackgroundMaterial; }
            set
            {
                _selectedBackgroundMaterial = value;
                UpdateStyle();
            }
        }



        [SerializeField]
        private bool _usePressedItemVisual = false;
        public bool UsePressedItemVisual
        {
            get { return _usePressedItemVisual; }
            set
            {
                _usePressedItemVisual = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Vector3 _pressedTextSize = new Vector3(10.5f, 10.5f, 5f);
        public Vector3 PressedTextSize
        {
            get { return _pressedTextSize; }
            set
            {
                _pressedTextSize = value;
                UpdateStyle();
            }
        }
        [SerializeField]
        private Material _pressedTextMaterial = null;
        public Material PressedTextMaterial
        {
            get { return _pressedTextMaterial; }
            set
            {
                _pressedTextMaterial = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Material _pressedBackgroundMaterial = null;
        public Material PressedBackgroundMaterial
        {
            get { return _pressedBackgroundMaterial; }
            set
            {
                _pressedBackgroundMaterial = value;
                UpdateStyle();
            }
        }

        public float holdPressedVisualFor = 0.15f;



        [SerializeField]
        private bool _useDisabledItemVisual = false;
        public bool UseDisabledItemVisual
        {
            get { return _useDisabledItemVisual; }
            set
            {
                _useDisabledItemVisual = value;
                UpdateStyle();
            }
        }

        [SerializeField]
        private Vector3 _disabledTextSize = new Vector3(9, 9, 1);
        public Vector3 DisabledTextSize
        {
            get { return _disabledTextSize; }
            set
            {
                _disabledTextSize = value;
                UpdateStyle();
            }
        }
        [SerializeField]
        private Material _disabledTextMaterial = null;
        public Material DisabledTextMaterial
        {
            get { return _disabledTextMaterial; }
            set
            {
                _disabledTextMaterial = value;
                UpdateStyle();
            }
        }
        [SerializeField]
        private Material _disabledBackgroundMaterial = null;
        public Material DisabledBackgroundMaterial
        {
            get { return _disabledBackgroundMaterial; }
            set
            {
                _disabledBackgroundMaterial = value;
                UpdateStyle();
            }
        }

        #endregion Style Options -------------------------------------------------------------------




        public bool useModules = true;
        public bool ignoreChildModules = false;

        public bool ignoreChildUnSelectModuleContainers = false;
        public bool applyUnSelectModuleContainers = true;
        public List<MText_ModuleContainer> unSelectModuleContainers = new List<MText_ModuleContainer>();

        public bool ignoreChildOnSelectModuleContainers = false;
        public bool applyOnSelectModuleContainers = true;
        public List<MText_ModuleContainer> onSelectModuleContainers = new List<MText_ModuleContainer>();

        public bool ignoreChildOnPressModuleContainers = false;
        public bool applyOnPressModuleContainers = true;
        public List<MText_ModuleContainer> onPressModuleContainers = new List<MText_ModuleContainer>();

        public bool ignoreChildOnClickModuleContainers = false;
        public bool applyOnClickModuleContainers = true;
        public List<MText_ModuleContainer> onClickModuleContainers = new List<MText_ModuleContainer>();

        public int selectedItem = 0;

        private float returnToSelectedTime = 0;
        bool pressed = false;
        bool selected = false; //used to check before press
        int counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;
        int previousSelection = 0;




        #region remember inspector layout
#if UNITY_EDITOR
        [HideInInspector] public bool showAnimationSettings = false;
        [HideInInspector] public bool showKeyboardSettings = false;
#endif
        #endregion remember inspector layout


        #region Unity things
        private void Awake()
        {
            if (!autoFocusOnStart)
            {
                this.enabled = false;
#if ENABLE_INPUT_SYSTEM
                if (GetComponent<PlayerInput>())
                    GetComponent<PlayerInput>().enabled = false;
#endif
                if (GetComponent<MText_InputSystemController>())
                    GetComponent<MText_InputSystemController>().enabled = false;

                return;
            }

            this.enabled = true;
#if ENABLE_INPUT_SYSTEM
            if (GetComponent<PlayerInput>())
                GetComponent<PlayerInput>().enabled = true;
#endif
            if (GetComponent<MText_InputSystemController>())
                GetComponent<MText_InputSystemController>().enabled = true;

            if (autoFocusFirstItem)
                SelectTheFirstSelectableItem();
            else
                UnselectEverything();


        }

        private void Update()
        {
            if (transform.childCount == 0)
                return;

            if (pressed)
            {
                if (Time.time > returnToSelectedTime)
                {
                    pressed = false;
                    if (transform.GetChild(selectedItem).GetComponent<MText_UI_Button>())
                        transform.GetChild(selectedItem).GetComponent<MText_UI_Button>().SelectedButtonVisualUpdate();
                }
            }
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            if (GetComponent<PlayerInput>())
                GetComponent<PlayerInput>().enabled = false;
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (GetComponent<MText_InputSystemController>())
                GetComponent<MText_InputSystemController>().enabled = false;
#endif
        }
        #endregion Unity things





        #region Public
        /// <summary>
        /// Updates the list positioning
        /// </summary>
        [ContextMenu("Update List")]
        public void UpdateList()
        {
            if (GetComponent<LayoutGroup>())
                GetComponent<LayoutGroup>().UpdateLayout();
        }




        /// <summary>
        /// Focuses/defocuses the list
        /// </summary>
        /// <param name="enable"></param>
        public void Focus(bool enable)
        {
            pressed = false;
            selected = false;

            if (enable)
            {

                if (autoFocusFirstItem)
                    SelectTheFirstSelectableItem();

                //UnselectEverything();//this was here
                UnselectEverythingExceptSelected();

                if (gameObject.activeInHierarchy)
                    StartCoroutine(FocusRoutine());
                else
                {
                    this.enabled = true;
#if ENABLE_INPUT_SYSTEM
                    if (GetComponent<PlayerInput>())
                        GetComponent<PlayerInput>().enabled = true;
#elif ENABLE_LEGACY_INPUT_MANAGER
                    if (GetComponent<MText_InputSystemController>())
                        GetComponent<MText_InputSystemController>().enabled = true;
#endif
                }
            }
            else
            {
                UnselectEverything();
                this.enabled = false;
#if ENABLE_INPUT_SYSTEM
                if (GetComponent<PlayerInput>())
                    GetComponent<PlayerInput>().enabled = false;
#elif ENABLE_LEGACY_INPUT_MANAGER
                if (GetComponent<MText_InputSystemController>())
                    GetComponent<MText_InputSystemController>().enabled = false;
#endif
            }
        }
        /// <summary>
        /// Focuses/defocus the list with a single frame delay if true is passed as second parameter
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="delay"></param>
        public void Focus(bool enable, bool delay)
        {
            pressed = false;
            selected = false;

            if (enable)
            {
                UnselectEverything();

                if (autoFocusFirstItem)
                    SelectTheFirstSelectableItem();

                if (delay && gameObject.activeInHierarchy)
                    StartCoroutine(FocusRoutine());
                else
                    this.enabled = true;
            }
            else
            {
                UnselectEverything();
                this.enabled = enable;
            }
        }
        /// <summary>
        /// Switches the focus mode
        /// </summary>
        [ContextMenu("Toggle Focus")]
        public void FocusToggle()
        {
            if (this.enabled)
                Focus(false, true);
            else
                Focus(true, true);
        }

        //coroutine to delay a single frame to avoid pressing "enter" key in one list to apply to another list just getting enabled
        private IEnumerator FocusRoutine()
        {
            yield return null;
            this.enabled = true;
#if ENABLE_INPUT_SYSTEM
            if (GetComponent<PlayerInput>())
                GetComponent<PlayerInput>().enabled = true;
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (GetComponent<MText_InputSystemController>())
                GetComponent<MText_InputSystemController>().enabled = true;
#endif
        }



        /// <summary>
        /// Processes the select item for the list. Doesn't let the item's components know it was selected
        /// Call the AlertSelectedItem(int) to update the list
        /// </summary>
        /// <param name="number"></param>
        public void SelectItem(int number)
        {
            if (transform.childCount > number)
            {
                selected = true;

                UpdateList();

                selectedItem = number;
            }
        }
        /// <summary>
        /// Alerts the list item that it was selected. Doesn't alert the list.
        /// Call the SelectItem(int) to update the list
        /// </summary>
        /// <param name="number"></param>
        public void AlertSelectedItem(int number)
        {
            if (transform.childCount > number)
            {
                Transform itemTransform = transform.GetChild(number);

                //Unity objects should not use null propagation. Remember this, just use if statement. - note to self
                if (itemTransform.GetComponent<MText_UI_Button>())
                    itemTransform.GetComponent<MText_UI_Button>().SelectButton();

                if (itemTransform.GetComponent<MText_UI_InputField>())
                    itemTransform.GetComponent<MText_UI_InputField>().Focus(true);

                if (itemTransform.GetComponent<MText_UI_Slider>())
                    itemTransform.GetComponent<MText_UI_Slider>().Focus(true);

                if (itemTransform.GetComponent<MText_UI_HorizontalSelector>())
                    itemTransform.GetComponent<MText_UI_HorizontalSelector>().Focus(true);
            }
        }



        public void UnselectItem(int i)
        {
            if (transform.childCount <= i || i < 0)
                return;

            if (transform.GetChild(i).GetComponent<MText_UI_Button>())
            {
                if (transform.GetChild(i).GetComponent<MText_UI_Button>().interactable)
                    transform.GetChild(i).GetComponent<MText_UI_Button>().UnselectButton();
                else
                    transform.GetChild(i).GetComponent<MText_UI_Button>().Uninteractable();
            }

            if (transform.GetChild(i).GetComponent<MText_UI_InputField>())
            {
                transform.GetChild(i).GetComponent<MText_UI_InputField>().Focus(false);
            }

            if (transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>())
            {
                if (transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>().interactable)
                    transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>().Focus(false);
                else
                    transform.GetChild(i).gameObject.GetComponent<MText_UI_Slider>().Uninteractable();
            }

            if (transform.GetChild(i).gameObject.GetComponent<MText_UI_HorizontalSelector>())
            {
                if (transform.GetChild(i).gameObject.GetComponent<MText_UI_HorizontalSelector>().interactable)
                    transform.GetChild(i).gameObject.GetComponent<MText_UI_HorizontalSelector>().Focus(false);
            }

        }

        public void UnselectEverything()
        {
            selectedItem = transform.childCount;
            UpdateList();
            for (int i = 0; i < transform.childCount; i++)
            {
                UnselectItem(i);
            }
        }

        /// <summary>
        /// Keeping the selected item value means the previously selected item can still be pressed after selected and scrolling via keyboard continues from previously selected one instead of starting from 0
        /// </summary>
        public void UnselectEverythingDontChangeSelectedItemValue()
        {
            UpdateList();
            for (int i = 0; i < transform.childCount; i++)
            {
                UnselectItem(i);
            }
        }

        public void UnselectEverythingExceptSelected()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i != selectedItem)
                    UnselectItem(i);
            }
        }

        public void PressSelectedItem()
        {
            if (selected)
                PresstItem(selectedItem);
        }

        public void PresstItem(int i)
        {
            if (transform.childCount > i)
            {
                pressed = true;

                //if (audioSource && itemSelectionSoundEffect)
                //    audioSource.PlayOneShot(itemSelectionSoundEffect);
                returnToSelectedTime = Time.time + holdPressedVisualFor;
                AlertPressedUIItem();
            }
        }

        /// <summary>
        /// Reapplies all variables/style choices, updating them.
        /// </summary>
        public void UpdateStyle()
        {
            //selectedItem = transform.childCount;
            UpdateList();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (selectedItem != i)
                    UnselectItem(i);
            }
            AlertSelectedItem(selectedItem);
        }
        #endregion Public




        #region Navigation
        private void Scrolled()
        {
            selected = true;
            SelectItem(selectedItem);
            AlertSelectedItem(selectedItem);

            if (selectedItem != previousSelection)
                UnselectItem(previousSelection);
        }
        public void ScrollUp()
        {
            previousSelection = selectedItem;

            selectedItem--;
            if (selectedItem < 0)
                selectedItem = transform.childCount - 1;

            while (!IsItemSelectable(selectedItem) && transform.childCount > 0 && counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop < transform.childCount)
            {
                counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop++;

                selectedItem--;
                if (selectedItem < 0)
                    selectedItem = transform.childCount - 1;
            }
            counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;

            Scrolled();
        }
        public void ScrollDown()
        {
            previousSelection = selectedItem;

            selectedItem++;
            if (selectedItem > transform.childCount - 1)
                selectedItem = 0;

            while (!IsItemSelectable(selectedItem) && transform.childCount > 0 && counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop < transform.childCount)
            {
                counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop++;

                selectedItem++;
                if (selectedItem > transform.childCount - 1)
                    selectedItem = 0;
            }
            counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;

            Scrolled();
        }

        public void ScrollLeft()
        {
            if (IsItemSelectable(selectedItem) && transform.childCount > 0)
            {
                if (transform.GetChild(selectedItem).GetComponent<MText_UI_HorizontalSelector>())
                    transform.GetChild(selectedItem).GetComponent<MText_UI_HorizontalSelector>().Decrease();
                else if (transform.GetChild(selectedItem).GetComponent<MText_UI_Slider>())
                    transform.GetChild(selectedItem).GetComponent<MText_UI_Slider>().DecreaseValue();
            }
        }
        public void ScrollRight()
        {
            if (IsItemSelectable(selectedItem) && transform.childCount > 0)
            {
                if (transform.GetChild(selectedItem).GetComponent<MText_UI_HorizontalSelector>())
                    transform.GetChild(selectedItem).GetComponent<MText_UI_HorizontalSelector>().Increase();
                else if (transform.GetChild(selectedItem).GetComponent<MText_UI_Slider>())
                    transform.GetChild(selectedItem).GetComponent<MText_UI_Slider>().IncreaseValue();
            }
        }
        #endregion Navigation



        //Used by ScrollList() method only
        private bool IsItemSelectable(int i)
        {
            if (transform.childCount > i)
            {
                Transform target = transform.GetChild(i);

                if (!target.gameObject.activeSelf)
                    return false;

                //it's a horizontal selector
                if (target.GetComponent<MText_UI_HorizontalSelector>())
                    return target.GetComponent<MText_UI_HorizontalSelector>().interactable;

                //it's a button
                else if (target.GetComponent<MText_UI_Button>())
                    return target.GetComponent<MText_UI_Button>().interactable;

                //it's a input Field
                else if (target.GetComponent<MText_UI_InputField>())
                    return target.GetComponent<MText_UI_InputField>().interactable;

                //it's a slider
                else if (target.gameObject.GetComponent<MText_UI_Slider>())
                    return target.GetComponent<MText_UI_Slider>().interactable;


            }

            return false;
        }

        private void AlertPressedUIItem()
        {
            if (transform.GetChild(selectedItem).GetComponent<MText_UI_Button>())
                transform.GetChild(selectedItem).GetComponent<MText_UI_Button>().PressButtonDontCallList();
        }

        private void SelectTheFirstSelectableItem()
        {
            selected = true;

            if (selectedItem > transform.childCount - 1)
                selectedItem = 0;

            while (!IsItemSelectable(selectedItem) && counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop < transform.childCount)
            {
                counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop++;

                selectedItem++;
                if (selectedItem > transform.childCount - 1)
                    selectedItem = 0;
            }
            counterToCheckIfAllItemsAreDisabledToAvoidInfiniteLoop = 0;

            SelectItem(selectedItem);
            AlertSelectedItem(selectedItem);
        }








        /// <summary>
        /// Create an empty effect and adds it to MText_ModuleContainer List
        /// </summary>
        public void EmptyEffect(List<MText_ModuleContainer> moduleList)
        {
            MText_ModuleContainer module = new MText_ModuleContainer();
            moduleList.Add(module);
        }


#if UNITY_EDITOR
        /// <summary>
        /// Editor only
        /// </summary>
        public void LoadDefaultSettings()
        {
            MText_Settings settings = EditorHelper.MText_FindResource.VerifySettings(null);

            if (settings)
            {
                NormalTextSize = settings.defaultListNormalTextSize;
                NormalTextMaterial = settings.defaultListNormalTextMaterial;
                NormalBackgroundMaterial = settings.defaultListNormalBackgroundMaterial;

                SelectedTextSize = settings.defaultListSelectedTextSize;
                SelectedTextMaterial = settings.defaultListSelectedTextMaterial;
                SelectedBackgroundMaterial = settings.defaultListSelectedBackgroundMaterial;

                PressedTextSize = settings.defaultListPressedTextSize;
                PressedTextMaterial = settings.defaultListPressedTextMaterial;
                PressedBackgroundMaterial = settings.defaultListPressedBackgroundMaterial;

                DisabledTextSize = settings.defaultListDisabledTextSize;
                DisabledTextMaterial = settings.defaultListDisabledTextMaterial;
                DisabledBackgroundMaterial = settings.defaultListDisabledBackgroundMaterial;
            }
        }
#endif
    }
}