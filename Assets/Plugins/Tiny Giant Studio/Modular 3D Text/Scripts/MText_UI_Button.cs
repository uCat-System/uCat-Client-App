using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MText
{
    /// <summary>
    /// The component for 3D buttons.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Modular 3D Text/Button (M3D)")]
    public class MText_UI_Button : MonoBehaviour
    {
        #region Variable Declaration
        /// <summary>
        /// Used to load default values.
        /// </summary>
        public MText_Settings settings = null;


        public UnityEvent onClick = new UnityEvent();
        public UnityEvent whileBeingClicked = null;
        public UnityEvent onSelect = null;
        public UnityEvent onUnselect = null;

        public bool interactable = true;
        [Tooltip("Mouse or touch can select this")]
        public bool interactableByMouse = true;

        [SerializeField] private Modular3DText _text;
        public Modular3DText Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    UpdateStyle();
                }
            }
        }


        [SerializeField] private Renderer _background;
        public Renderer Background
        {
            get
            {
                return _background;
            }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    UpdateStyle();
                }
            }
        }

        #region Module
        public bool useModules = true;
        public bool applyOnSelectModuleContainers = true;
        public List<MText_ModuleContainer> onSelectModuleContainers = new List<MText_ModuleContainer>();

        public bool applyUnSelectModuleContainers = true;
        public List<MText_ModuleContainer> unSelectModuleContainers = new List<MText_ModuleContainer>();

        public bool applyOnPressModuleContainers = true;
        public List<MText_ModuleContainer> onPressModuleContainers = new List<MText_ModuleContainer>();

        public bool applyOnClickModuleContainers = true;
        public List<MText_ModuleContainer> onClickModuleContainers = new List<MText_ModuleContainer>();
        #endregion Module


        #region Style variables
        public bool useStyles = true;

        [SerializeField] private Vector3 _normalTextSize = new Vector3(8, 8, 1);
        public Vector3 NormalTextSize
        {
            get
            {
                return _normalTextSize;
            }
            set
            {
                if (_normalTextSize != value)
                {
                    _normalTextSize = value;
                    UpdateStyle();
                }

            }
        }
        [SerializeField] private Material _normalTextMaterial = null;
        public Material NormalTextMaterial
        {
            get
            {
                return _normalTextMaterial;
            }
            set
            {
                if (_normalTextMaterial != value)
                {
                    _normalTextMaterial = value;
                    UpdateStyle();
                }
            }
        }

        [SerializeField] private Material _normalBackgroundMaterial = null;
        public Material NormalBackgroundMaterial
        {
            get
            {
                return _normalBackgroundMaterial;
            }
            set
            {
                if (_normalBackgroundMaterial != value)
                {
                    _normalBackgroundMaterial = value;
                    UpdateStyle();
                }
            }
        }

        public bool useSelectedVisual = true;

        [SerializeField] private Vector3 _selectedTextSize = new Vector3(8.2f, 8.2f, 1);
        public Vector3 SelectedTextSize
        {
            get
            {
                return _selectedTextSize;
            }
            set
            {
                if (_selectedTextSize != value)
                {
                    _selectedTextSize = value;
                    UpdateStyle();
                }
            }
        }

        [SerializeField] private Material _selectedTextMaterial = null;
        public Material SelectedTextMaterial
        {
            get
            {
                return _selectedTextMaterial;
            }
            set
            {
                if (_selectedTextMaterial != value)
                {
                    _selectedTextMaterial = value;
                    UpdateStyle();
                }
            }
        }

        [SerializeField] private Material _selectedBackgroundMaterial = null;
        public Material SelectedBackgroundMaterial
        {
            get
            {
                return _selectedBackgroundMaterial;
            }
            set
            {
                if (_selectedBackgroundMaterial != value)
                {
                    _selectedBackgroundMaterial = value;
                    UpdateStyle();
                }
            }
        }


        public bool usePressedVisual = true;

        [SerializeField] private Vector3 _pressedTextSize = new Vector3(8.5f, 8.5f, 1);
        public Vector3 PressedTextSize
        {
            get
            {
                return _pressedTextSize;
            }
            set
            {
                if (_pressedTextSize != value)
                {
                    _pressedTextSize = value;
                    UpdateStyle();
                }
            }
        }

        [SerializeField] private Material _pressedTextMaterial = null;
        public Material PressedTextMaterial
        {
            get
            {
                return _pressedTextMaterial;
            }
            set
            {
                if (_pressedTextMaterial != value)
                {
                    _pressedTextMaterial = value;
                    UpdateStyle();
                }
            }
        }


        [SerializeField] private Material _pressedBackgroundMaterial = null;
        public Material PressedBackgroundMaterial
        {
            get
            {
                return _pressedBackgroundMaterial;
            }
            set
            {
                if (_pressedBackgroundMaterial != value)
                {
                    _pressedBackgroundMaterial = value;
                    UpdateStyle();
                }
            }
        }
        public float holdPressedVisualFor = 0.15f;



        #region Disabled
        [SerializeField] private bool _useDisabledVisual = true;
        public bool UseDisabledVisual
        {
            get { return _useDisabledVisual; }
            set { _useDisabledVisual = value; UpdateStyle(); }
        }

        [SerializeField] Vector3 _disabledTextSize = new Vector3(8, 8, 8);
        public Vector3 DisabledTextSize
        {
            get { return _disabledTextSize; }
            set { _disabledTextSize = value; UpdateStyle(); }
        }

        [SerializeField] Material _disabledTextMaterial = null;
        public Material DisabledTextMaterial
        {
            get { return _disabledTextMaterial; }
            set { _disabledTextMaterial = value; UpdateStyle(); }
        }

        [SerializeField] Material _disabledBackgroundMaterial = null;
        public Material DisabledBackgroundMaterial
        {
            get { return _disabledBackgroundMaterial; }
            set { _disabledBackgroundMaterial = value; UpdateStyle(); }
        }
        #endregion Disabled


        public bool isSelected = false;
        #endregion Style



        #region remember inspector layout
#if UNITY_EDITOR
        [HideInInspector] public bool showEvents = false;
        [HideInInspector] public bool hideOverwrittenVariablesFromInspector;
#endif
        #endregion remember inspector layout

        #endregion Variable Declaration







        private void Awake()
        {
            this.enabled = false;
        }



        public void UpdateStyle()
        {
            if (!interactable)
                DisabledButtonVisualUpdate();
            else if (isSelected)
                SelectedButtonVisualUpdate();
            else
                UnselectedButtonVisualUpdate();
        }

        /// <summary>
        /// call this to select a button
        /// </summary>
        public void SelectButton()
        {
            MText_UI_List parentList = MText_Utilities.GetParentList(transform);
            if (parentList)
            {
                int childNumber = transform.GetSiblingIndex();
                parentList.UnselectItem(parentList.selectedItem);
                parentList.SelectItem(childNumber);
            }

            SelectedButtonVisualUpdate();
            SelectedButtonModuleUpdate();
            onSelect.Invoke();
        }
        public void SelectedButtonVisualUpdate()
        {
            isSelected = true;
            var applyOnSelectStyle = ApplyOnSelectStyle();

            if (applyOnSelectStyle.Item1)
            {
                ApplyeStyle(applyOnSelectStyle.Item3.SelectedTextSize, applyOnSelectStyle.Item3.SelectedTextMaterial, applyOnSelectStyle.Item3.SelectedBackgroundMaterial);
            }
            else if (applyOnSelectStyle.Item2)
            {
                ApplyeStyle(SelectedTextSize, SelectedTextMaterial, SelectedBackgroundMaterial);
            }
        }
        public void SelectedButtonModuleUpdate()
        {
            (bool, bool, MText_UI_List) applyModules = ApplyOnSelectModule();

            //list modules
            if (applyModules.Item1)
                CallModules(applyModules.Item3.onSelectModuleContainers);
            //self modules
            if (applyModules.Item2)
                CallModules(onSelectModuleContainers);
        }

        /// <summary>
        /// Call this to unselect a button
        /// </summary>
        public void UnselectButton()
        {
            UnselectedButtonVisualUpdate();
#if UNITY_EDITOR    
            if (EditorApplication.isPlaying)
            {
                UnselectButtonModuleUpdate();
                onUnselect.Invoke();
            }
#else
            UnselectButtonModuleUpdate();
            onUnselect.Invoke();
#endif
        }
        public void UnselectedButtonVisualUpdate()
        {
            isSelected = false;

            //apply from list
            if (ApplyNormalStyle().Item1)
            {
                MText_UI_List parent = MText_Utilities.GetParentList(transform);
                ApplyeStyle(parent.NormalTextSize, parent.NormalTextMaterial, parent.NormalBackgroundMaterial);
            }
            else if (ApplyNormalStyle().Item2)
            {
                ApplyeStyle(NormalTextSize, NormalTextMaterial, NormalBackgroundMaterial);
            }
        }
        public void UnselectButtonModuleUpdate()
        {
            (bool, bool, MText_UI_List) applyModules = ApplyUnSelectModule();

            //list modules
            if (applyModules.Item1)
                CallModules(applyModules.Item3.unSelectModuleContainers);
            //self modules
            if (applyModules.Item2)
                CallModules(unSelectModuleContainers);
        }
        float switchBackVisualTime;
        /// <summary>
        /// Used everywhere except list
        /// </summary>
        public void PressButton()
        {
            MText_UI_List parentList = MText_Utilities.GetParentList(transform);
            if (parentList)
            {
                parentList.PresstItem(transform.GetSiblingIndex());
            }
            else
            {
                switchBackVisualTime = Time.time + holdPressedVisualFor;
                Invoke(nameof(ReturnToNormalVisualAfterPressing), holdPressedVisualFor);
            }
            PressButtonVisualUpdate();
            onClick.Invoke();
            OnClickButtonModuleUpdate();
        }
        void ReturnToNormalVisualAfterPressing()
        {
            if (Time.time <= switchBackVisualTime)
            {
                if (isSelected)
                    SelectedButtonVisualUpdate();
                else
                    UnselectedButtonVisualUpdate();
            }
        }

        /// <summary>
        /// This is called from list. This avoids recursion.
        /// </summary>
        public void PressButtonDontCallList()
        {
            PressButtonVisualUpdate();
            OnClickButtonModuleUpdate();
            onClick.Invoke();
        }

        //Difference between PressButtonClick() & PressButtonVisualUpdate() is that PressButtonVisualUpdate() automatically returns to selected visual
        //used by List
        public void PressButtonVisualUpdate()
        {
            //item 1 = apply parentstyle, item2 = apply selfstyle, item3 = list
            var applyPressedStyle = ApplyPressedStyle();

            if (applyPressedStyle.Item1)
            {
                ApplyeStyle(applyPressedStyle.Item3.PressedTextSize, applyPressedStyle.Item3.PressedTextMaterial, applyPressedStyle.Item3.PressedBackgroundMaterial);
            }
            else if (applyPressedStyle.Item2)
            {
                ApplyeStyle(PressedTextSize, PressedTextMaterial, PressedBackgroundMaterial);
            }
        }

        //the methods above this line for pressed are called by everything except raycaster

        //Difference between PressButtonClick() & PressButtonVisualUpdate() is that PressButtonVisualUpdate() automatically returns to selected visual
        //used by Raycaster
        public void PressButtonClick()
        {
            (bool, bool, MText_UI_List) applyPressedStyle = ApplyPressedStyle();

            if (applyPressedStyle.Item1)
            {
                ApplyeStyle(applyPressedStyle.Item3.PressedTextSize, applyPressedStyle.Item3.PressedTextMaterial, applyPressedStyle.Item3.PressedBackgroundMaterial);
            }
            else if (applyPressedStyle.Item2)
            {
                ApplyeStyle(PressedTextSize, PressedTextMaterial, PressedBackgroundMaterial);
            }
        }
        public void ButtonBeingPressed()
        {
            whileBeingClicked.Invoke();
            OnPressButtonModuleUpdate();
        }
        public void PressedButtonClickStopped()
        {
            if (isSelected)
            {
                onClick.Invoke();
                OnClickButtonModuleUpdate();
            }

            if (isSelected)
                SelectedButtonVisualUpdate();
            else
                UnselectedButtonVisualUpdate();
        }

        public void OnClickButtonModuleUpdate()
        {
            (bool, bool, MText_UI_List) applyModules = ApplyOnClickModule();

            //list modules
            if (applyModules.Item1)
                CallModules(applyModules.Item3.onClickModuleContainers);
            //self modules
            if (applyModules.Item2)
                CallModules(onClickModuleContainers);
        }
        public void OnPressButtonModuleUpdate()
        {
            (bool, bool, MText_UI_List) applyModules = ApplyOnPresstModule();

            //list modules
            if (applyModules.Item1)
                CallModules(applyModules.Item3.onPressModuleContainers);
            //self modules
            if (applyModules.Item2)
                CallModules(onPressModuleContainers);
        }

        public void DisabledButtonVisualUpdate()
        {
            //item1 : false = apply from parent & //true = apply from self. item2 = parent
            var applyDisabledStyle = ApplyDisabledStyle();

            //Apply from parent
            if (applyDisabledStyle.Item1)
                ApplyeStyle(applyDisabledStyle.Item3.DisabledTextSize, applyDisabledStyle.Item3.DisabledTextMaterial, applyDisabledStyle.Item3.DisabledBackgroundMaterial);
            //Apply self
            else if (applyDisabledStyle.Item2)
                ApplyeStyle(DisabledTextSize, DisabledTextMaterial, DisabledBackgroundMaterial);
            //Dont do anything
        }


        private void ApplyeStyle(Vector3 fontSize, Material fontMat, Material backgroundMat)
        {
            //if (!useStyles)
            //    return;

            if (Text)
            {
                Text.FontSize = fontSize;
                Text.Material = fontMat;
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Text.UpdateText();
#else
                Text.UpdateText();
#endif
            }

            if (Background)
            {
                Background.material = backgroundMat;
            }
        }

        /// <summary>
        /// Call this to set the button as Uninteractable
        /// </summary>
        public void Uninteractable()
        {
            interactable = false;
            DisabledButtonVisualUpdate();
        }
        /// <summary>
        /// Call this to set the button as Interactable
        /// </summary>
        public void Interactable()
        {
            interactable = true;
            UnselectedButtonVisualUpdate();
        }




        //these checks are public for the editorscript only
        #region Check if style should be applied from here
        //first is apply from list
        public (bool, bool) ApplyNormalStyle()
        {
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.UseStyle && list.UseNormalItemVisual)
                {
                    return (true, false);
                }
            }
            //don't apply from list
            if (useStyles)
                return (false, true);

            //don't get style
            return (false, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Apply from perent, apply from self, list</returns>
        public (bool, bool, MText_UI_List) ApplyOnSelectStyle()
        {
            //get style from parent list
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.UseStyle && list.UseSelectedItemVisual)
                {
                    return (true, false, list);
                }
            }
            //get style from itself
            if (useStyles && useSelectedVisual)
                return (false, true, null);

            //don't get style
            return (false, false, null);
        }
        //item1 = parent, item2 = self
        public (bool, bool, MText_UI_List) ApplyPressedStyle()
        {
            //get style from parent list
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.UseStyle && list.UsePressedItemVisual)
                    return (true, false, list);
            }
            //get style from itself
            if (useStyles && usePressedVisual)
                return (false, true, null);

            return (false, false, null);
        }

        /// <summary>
        /// Item 1 = apply from parent
        /// Item 2 = apply from self
        /// </summary>
        /// <returns></returns>
        //false = apply from parent & //true = apply from self
        public (bool, bool, MText_UI_List) ApplyDisabledStyle()
        {
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.UseStyle && list.UseDisabledItemVisual)
                    return (true, false, list);
            }
            if (useStyles && UseDisabledVisual)
                return (false, true, null);

            return (false, false, null);
        }
        #endregion Check if style should be applied ends here



        private void CallModules(List<MText_ModuleContainer> moduleContainers)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (moduleContainers.Count > 0)
            {
                for (int i = 0; i < moduleContainers.Count; i++)
                {
                    if (moduleContainers[i].module)
                        StartCoroutine(moduleContainers[i].module.ModuleRoutine(gameObject, moduleContainers[i].variableHolders));
                }
            }
        }

        #region Check if Module should be applied from here
        public (bool, bool, MText_UI_List) ApplyUnSelectModule()
        {
            bool applySelfModules = false;
            bool applyListModule = false;
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.useModules && list.applyOnSelectModuleContainers)
                    applyListModule = true;
                if (list.ignoreChildModules || list.ignoreChildUnSelectModuleContainers)
                    return (applyListModule, applySelfModules, list);
            }

            if (useModules && applyUnSelectModuleContainers)
                applySelfModules = true;

            return (applyListModule, applySelfModules, list);
        }
        public (bool, bool, MText_UI_List) ApplyOnPresstModule()
        {
            bool applySelfModules = false;
            bool applyListModule = false;
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.useModules && list.applyOnPressModuleContainers)
                    applyListModule = true;
                if (list.ignoreChildModules || list.ignoreChildOnPressModuleContainers)
                    return (applyListModule, applySelfModules, list);
            }
            if (useModules && applyOnPressModuleContainers)
                applySelfModules = true;

            return (applyListModule, applySelfModules, list);
        }
        public (bool, bool, MText_UI_List) ApplyOnClickModule()
        {
            bool applySelfModules = false;
            bool applyListModule = false;
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.useModules && list.applyOnClickModuleContainers)
                    applyListModule = true;
                if (list.ignoreChildModules || list.ignoreChildOnClickModuleContainers)
                    return (applyListModule, applySelfModules, list);
            }
            if (useModules && applyOnPressModuleContainers)
                applySelfModules = true;

            return (applyListModule, applySelfModules, list);
        }
        public (bool, bool, MText_UI_List) ApplyOnSelectModule()
        {
            bool applySelfModules = false;
            bool applyListModule = false;
            MText_UI_List list = MText_Utilities.GetParentList(transform);
            if (list)
            {
                if (list.useModules && list.applyOnSelectModuleContainers)
                    applyListModule = true;
                if (list.ignoreChildModules || list.ignoreChildOnSelectModuleContainers)
                    return (applyListModule, applySelfModules, list);
            }

            if (useModules && applyOnSelectModuleContainers)
                applySelfModules = true;

            return (applyListModule, applySelfModules, list);
        }
        #endregion Check if Module should be applied ends here

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
            if (settings == null)
                settings = EditorHelper.MText_FindResource.VerifySettings(null);

            if (settings)
            {
                NormalTextSize = settings.defaultButtonNormalTextSize;
                NormalTextMaterial = settings.defaultButtonNormalTextMaterial;
                NormalBackgroundMaterial = settings.defaultButtonNormalBackgroundMaterial;

                SelectedTextSize = settings.defaultButtonSelectedTextSize;
                SelectedTextMaterial = settings.defaultButtonSelectedTextMaterial;
                SelectedBackgroundMaterial = settings.defaultButtonSelectedBackgroundMaterial;

                PressedTextSize = settings.defaultButtonPressedTextSize;
                PressedTextMaterial = settings.defaultButtonPressedTextMaterial;
                PressedBackgroundMaterial = settings.defaultButtonPressedBackgroundMaterial;

                DisabledTextSize = settings.defaultButtonDisabledTextSize;
                DisabledTextMaterial = settings.defaultButtonDisabledTextMaterial;
                DisabledBackgroundMaterial = settings.defaultButtonDisabledBackgroundMaterial;

            }
        }
#endif
    }
}