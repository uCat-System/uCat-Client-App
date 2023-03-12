/// Created by Ferdowsur Asif @ Tiny Giant Studio

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MText
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Modular 3D Text/Input field (M3D)")]
    public class MText_UI_InputField : MonoBehaviour //Damm. Noticed MText was written as Mtext but can't change it now without causing users to lose their serialized fields
    {

        [Tooltip("if not in a list")]
        public bool autoFocusOnGameStart = true;
        public bool interactable = true;

        public int maxCharacter = 20;
        public string typingSymbol = "|";
        public bool hideTypingSymbolIfMaxCharacter = true;

        public bool passwordMode = false;
        public string replaceWith = "*";

        [SerializeField]
        private string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set { _text = value; UpdateText(true); }
        }

        public string placeHolderText = "Enter Text...";

        public Modular3DText textComponent = null;
        public Renderer background = null;

        public bool enterKeyEndsInput = true;

        public Material placeHolderTextMat = null;

        public Material inFocusTextMat = null;
        public Material inFocusBackgroundMat = null;

        public Material outOfFocusTextMat = null;
        public Material outOfFocusBackgroundMat = null;

        public Material disabledTextMat = null;
        public Material disabledBackgroundMat = null;

        private Material currentTextMaterial = null;

        [SerializeField]
        private AudioClip typeSound = null;
        [SerializeField]
        private AudioSource audioSource = null;

        public UnityEvent onInput = null;
        public UnityEvent onBackspace = null;
        public UnityEvent onInputEnd = null;


        #region remember inspector layout
#if UNITY_EDITOR
        [HideInInspector] public bool showMainSettings = true;
        [HideInInspector] public bool showStyleSettings = false;
        [HideInInspector] public bool showAudioSettings = false;
        [HideInInspector] public bool showUnityEventSettings = false;
#endif
        #endregion remember inspector layout

        public string test;

        private void Awake()
        {
            if (!MText_Utilities.GetParentList(transform))
                Focus(autoFocusOnGameStart);
        }

#if ENABLE_INPUT_SYSTEM
        protected void OnEnable()
        {
            Keyboard.current.onTextInput += OnTextInput;
        }

        protected void OnDisable()
        {
            Keyboard.current.onTextInput -= OnTextInput;
        }

        void OnTextInput(char ch)
        {
            if (!interactable)
                return;
            ProcessNewChar(ch);
        }
#else
        private void Update()
        {
            if (!interactable)
                return;

            foreach (char c in Input.inputString)
            {
                ProcessNewChar(c);
            }
        }
#endif

        private void ProcessNewChar(char c)
        {
            if (c == '\b') // has backspace/delete been pressed?
            {
                if (_text.Length != 0)
                {
                    _text = _text.Substring(0, _text.Length - 1);
                    UpdateText(true);
                    onBackspace.Invoke();
                }
            }
            else if (((c == '\n') || (c == '\r')) && enterKeyEndsInput) // enter/return
            {
                InputComplete();
            }
            else
            {
                if (_text.Length < maxCharacter)
                {
                    _text += c;
                    UpdateText(true);
                    onInput.Invoke();
                }
            }
        }

        public void InputComplete()
        {
            onInputEnd.Invoke();
            this.enabled = false;
        }

        public void UpdateText()
        {
            UpdateText(false);
        }
        public void UpdateText(string newText)
        {
            _text = newText;
            UpdateText(false);
        }
        public void UpdateText(int newTextInt)
        {
            _text = newTextInt.ToString();
            UpdateText(false);
        }
        public void UpdateText(float newTextFloat)
        {
            _text = newTextFloat.ToString();
            UpdateText(false);
        }

        public void UpdateText(bool sound)
        {
            if (!textComponent)
                return;

            TouchScreenKeyboard.Open(_text);


            if (!string.IsNullOrEmpty(_text))
            {
                textComponent.Material = currentTextMaterial;
                GetText();
            }
            else
            {
                textComponent.Material = placeHolderTextMat;
                textComponent.Text = placeHolderText;
            }

            if (typeSound && sound && audioSource)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(typeSound);
            }
        }

        private void GetText()
        {
            if (_text.Length >= maxCharacter && hideTypingSymbolIfMaxCharacter)
                textComponent.Text = ProcessedInput();
            else
                textComponent.Text = string.Concat(ProcessedInput(), typingSymbol);
        }

        string ProcessedInput()
        {
            if (!passwordMode)
                return _text;

            string newText = "";
            foreach (char c in _text)
                newText += replaceWith;

            return newText;
        }

        /// <summary>
        /// Sets the text to empty
        /// </summary>
        public void EmptyText()
        {
            _text = string.Empty;
            UpdateText(false);
        }

        public void Select()
        {
            Focus(true);

            if (transform.parent)
            {
                if (transform.parent.GetComponent<MText_UI_List>())
                    transform.parent.GetComponent<MText_UI_List>().SelectItem(transform.GetSiblingIndex());
            }
        }

        //coroutine to delay a single frame to avoid pressing "enter" key in one list to apply to another UI just getting enabled
        public void Focus(bool enable)
        {
            StartCoroutine(FocusRoutine(enable));
        }

        //coroutine to delay a single frame to avoid pressing "enter" key in one list to apply to another UI just getting enabled
        IEnumerator FocusRoutine(bool enable)
        {
            yield return null;
            FocusFunction(enable);
        }

        public void Focus(bool enable, bool delay)
        {
            if (!delay)
                FocusFunction(enable);
            else
                StartCoroutine(FocusRoutine(enable));
        }

        void FocusFunction(bool enable)
        {
            if (interactable)
            {
                this.enabled = enable;

                if (enable)
                    SelectedVisual();
                else
                    UnselectedVisual();
            }
            else
            {
                DisableVisual();
            }

            UpdateText(false);
        }


        public void Interactable()
        {
            Focus(false, false);
        }
        public void Uninteractable()
        {
            this.enabled = false;

            DisableVisual();
            UpdateText(false);
        }

#if UNITY_EDITOR
        public void InteractableUsedByEditorOnly()
        {
            FocusFunction(true);
        }

        public void UninteractableUsedByEditorOnly()
        {
            this.enabled = false;

            DisableVisual();
            UpdateText(false);
        }
#endif




        #region Visuals
        private void SelectedVisual()
        {
            //item1 = applyStyleFromParent
            var applySelectedItemMaterial = ApplySelectedStyleFromParent();

            //apply parent list mat
            if (applySelectedItemMaterial.Item1)
                UpdateMaterials(applySelectedItemMaterial.Item2.SelectedTextMaterial, applySelectedItemMaterial.Item2.SelectedBackgroundMaterial);
            //apply self mat
            else
                UpdateMaterials(inFocusTextMat, inFocusBackgroundMat);
        }

        private void UnselectedVisual()
        {
            var applyNormalStyle = ApplyNormalStyleFromParent();

            //apply parent list mat
            if (applyNormalStyle.Item1)
            {
                Material textMat = string.IsNullOrEmpty(_text) ? placeHolderTextMat : applyNormalStyle.Item2.NormalTextMaterial;
                UpdateMaterials(textMat, applyNormalStyle.Item2.NormalBackgroundMaterial);
            }
            //apply self mat
            else
            {
                Material textMat = string.IsNullOrEmpty(_text) ? placeHolderTextMat : outOfFocusTextMat;
                UpdateMaterials(textMat, outOfFocusBackgroundMat);
            }
        }

        public void DisableVisual()
        {
            var applySelectedItemMaterial = ApplyDisabledStyleFromParent();

            //apply parent list mat
            if (applySelectedItemMaterial.Item1)
            {
                Material textMat = string.IsNullOrEmpty(_text) ? placeHolderTextMat : applySelectedItemMaterial.Item2.DisabledTextMaterial;
                UpdateMaterials(textMat, applySelectedItemMaterial.Item2.DisabledBackgroundMaterial);
            }
            //apply self mat
            else
            {
                Material textMat = string.IsNullOrEmpty(_text) ? placeHolderTextMat : disabledTextMat;
                UpdateMaterials(textMat, disabledBackgroundMat);
            }
        }

        //here
        private void UpdateMaterials(Material textMat, Material backgroundMat)
        {
            if (textComponent)
                textComponent.Material = textMat;
            if (background)
                background.material = backgroundMat;

            currentTextMaterial = textMat;
        }

        private MText_UI_List GetParentList()
        {
            return MText_Utilities.GetParentList(transform);
        }

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
        #endregion Visual
    }
}
