using UnityEditor;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(MText_UI_InputField))]
    public class MText_UI_InputFieldEditor : Editor
    {
        public MText_Settings settings;

        readonly float defaultSmallHorizontalFieldSize = 72.5f;
        readonly float defaultNormalltHorizontalFieldSize = 100;
        readonly float defaultLargeHorizontalFieldSize = 120f;
        readonly float defaultExtraLargeHorizontalFieldSize = 150f;


        MText_UI_InputField myTarget;
        SerializedObject soTarget;

        SerializedProperty autoFocusOnGameStart;
        SerializedProperty interactable;

        SerializedProperty maxCharacter;
        SerializedProperty typingSymbol;
        SerializedProperty hideTypingSymbolIfMaxCharacter;

        SerializedProperty enterKeyEndsInput;

        SerializedProperty passwordMode;
        SerializedProperty replaceWith;


        SerializedProperty textComponent;
        SerializedProperty background;

        SerializedProperty text;
        SerializedProperty placeHolderText;

        SerializedProperty placeHolderTextMat;

        SerializedProperty inFocusTextMat;
        SerializedProperty inFocusBackgroundMat;

        SerializedProperty outOfFocusTextMat;
        SerializedProperty outOfFocusBackgroundMat;

        SerializedProperty disabledTextMat;
        SerializedProperty disabledBackgroundMat;

        SerializedProperty typeSound;
        SerializedProperty audioSource;

        SerializedProperty onInput;
        SerializedProperty onBackspace;
        SerializedProperty onInputEnd;

        bool showMainSettings = true;
        bool showStyleSettings = false;
        bool showAudioSettings = false;
        bool showUnityEventSettings = false;

        GUIStyle foldOutStyle;

        enum FieldSize
        {
            small,
            normal,
            large,
            extraLarge
        }


        private void OnEnable()
        {
            myTarget = (MText_UI_InputField)target;
            soTarget = new SerializedObject(target);

            autoFocusOnGameStart = soTarget.FindProperty("autoFocusOnGameStart");
            interactable = soTarget.FindProperty("interactable");

            maxCharacter = soTarget.FindProperty("maxCharacter");
            typingSymbol = soTarget.FindProperty("typingSymbol");
            hideTypingSymbolIfMaxCharacter = soTarget.FindProperty("hideTypingSymbolIfMaxCharacter");
            enterKeyEndsInput = soTarget.FindProperty("enterKeyEndsInput");

            passwordMode = soTarget.FindProperty("passwordMode");
            replaceWith = soTarget.FindProperty("replaceWith");

            textComponent = soTarget.FindProperty("textComponent");
            background = soTarget.FindProperty("background");

            text = soTarget.FindProperty("_text");
            placeHolderText = soTarget.FindProperty("placeHolderText");


            placeHolderTextMat = soTarget.FindProperty("placeHolderTextMat");

            inFocusTextMat = soTarget.FindProperty("inFocusTextMat");
            inFocusBackgroundMat = soTarget.FindProperty("inFocusBackgroundMat");

            outOfFocusTextMat = soTarget.FindProperty("outOfFocusTextMat");
            outOfFocusBackgroundMat = soTarget.FindProperty("outOfFocusBackgroundMat");

            disabledTextMat = soTarget.FindProperty("disabledTextMat");
            disabledBackgroundMat = soTarget.FindProperty("disabledBackgroundMat");


            disabledBackgroundMat = soTarget.FindProperty("disabledBackgroundMat");

            typeSound = soTarget.FindProperty("typeSound");
            audioSource = soTarget.FindProperty("audioSource");

            onInput = soTarget.FindProperty("onInput");
            onBackspace = soTarget.FindProperty("onBackspace");
            onInputEnd = soTarget.FindProperty("onInputEnd");


            showMainSettings = myTarget.showMainSettings;
            showStyleSettings = myTarget.showStyleSettings;
            showAudioSettings = myTarget.showAudioSettings;
            showUnityEventSettings = myTarget.showUnityEventSettings;
        }

        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            MainSettings();
            StyleSettings();
            AudioSettings();
            UnityEventsSettings();

            SaveInspectorLayoutSettings();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();

                ApplyModifiedValuesToGraphics();

                EditorUtility.SetDirty(myTarget);
            }
        }

        void MainSettings()
        {
            //GUILayout.BeginVertical("Box");
            //EditorGUI.indentLevel = 1;
            //GUIContent content = new GUIContent("Main Settings");
            //showMainSettings = EditorGUILayout.Foldout(showMainSettings, content, true, EditorStyles.foldout);
            //if (showMainSettings)
            {
                EditorGUI.indentLevel = 0;

                if (!MText_Utilities.GetParentList(myTarget.transform))
                    HorizontalField(autoFocusOnGameStart, "Auto Focus", "This focuses the element on game start. Focused = you can type to give input.");

                HorizontalField(interactable, "Interactable");

                EditorGUILayout.Space(10);

                HorizontalField(text, "Text");
                HorizontalField(placeHolderText, "Placeholder");
                HorizontalField(maxCharacter, "Max Character");
                HorizontalField(typingSymbol, "Typing Symbol");
                HorizontalField(hideTypingSymbolIfMaxCharacter, "Hide Symbol if max char", "This hides the typing symbol when max character has been typed.", FieldSize.extraLarge);
                HorizontalField(enterKeyEndsInput, "Enter Key Ends Input", "", FieldSize.extraLarge);

                HorizontalField(passwordMode, "Password Mode");
                if (myTarget.passwordMode)
                {
                    EditorGUI.indentLevel = 1;
                    HorizontalField(replaceWith, "Replace with");
                    EditorGUI.indentLevel = 0;
                }
                //DrawUILine(Color.grey, 1, 2);

                EditorGUILayout.Space(10);

                if (!myTarget.textComponent)
                    EditorGUILayout.HelpBox("Text Component is required", MessageType.Error);

                HorizontalField(textComponent, "Text Component", "Reference to the 3D Text where input will be shown");
                HorizontalField(background, "Background");
                EditorGUILayout.Space(10);
            }
            //if (!Selection.activeTransform)
            //{
            //    showMainSettings = false;
            //}
            //GUILayout.EndVertical();
        }
        void StyleSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Style Settings");
            showStyleSettings = EditorGUILayout.Foldout(showStyleSettings, content, true, foldOutStyle);
            GUILayout.EndVertical();
            if (showStyleSettings)
            {
                EditorGUI.indentLevel = 0;

                EditorGUILayout.Space(10);
                //DrawUILine(Color.grey, 1, 2);

                EditorGUILayout.LabelField("Font Metarial");
                EditorGUI.indentLevel = 1;
                HorizontalField(placeHolderTextMat, "Placeholder");
                HorizontalField(inFocusTextMat, "In Focus");
                HorizontalField(outOfFocusTextMat, "Out of Focus");
                HorizontalField(disabledTextMat, "Disabled");

                EditorGUILayout.Space(10);
                EditorGUI.indentLevel = 0;
                EditorGUILayout.LabelField("Background Metarial");
                EditorGUI.indentLevel = 1;
                HorizontalField(inFocusBackgroundMat, "In Focus");
                HorizontalField(outOfFocusBackgroundMat, "Out of Focus");
                HorizontalField(disabledBackgroundMat, "Disabled");
            }
            if (!Selection.activeTransform)
            {
                showStyleSettings = false;
            }
            GUILayout.EndVertical();
        }
        void AudioSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Audio Settings");
            showAudioSettings = EditorGUILayout.Foldout(showAudioSettings, content, true, foldOutStyle);
            GUILayout.EndVertical();
            if (showAudioSettings)
            {
                EditorGUI.indentLevel = 0;

                //DrawUILine(Color.grey, 1, 2);

                HorizontalField(typeSound, "Type Sound");
                HorizontalField(audioSource, "Audio Source");
            }
            if (!Selection.activeTransform)
            {
                showAudioSettings = false;
            }
            GUILayout.EndVertical();
        }

        void UnityEventsSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Unity Events");
            showUnityEventSettings = EditorGUILayout.Foldout(showUnityEventSettings, content, true, foldOutStyle);
            GUILayout.EndVertical();
            if (showUnityEventSettings)
            {
                EditorGUILayout.PropertyField(onInput);
                EditorGUILayout.PropertyField(onBackspace);
                EditorGUILayout.PropertyField(onInputEnd);
            }
            if (!Selection.activeTransform)
            {
                showUnityEventSettings = false;
            }
            GUILayout.EndVertical();
        }

        void ApplyModifiedValuesToGraphics()
        {
            if (!myTarget.interactable)
                myTarget.UninteractableUsedByEditorOnly();
            else
                myTarget.InteractableUsedByEditorOnly();
        }



        private void SaveInspectorLayoutSettings()
        {
            myTarget.showMainSettings = showMainSettings;
            myTarget.showStyleSettings = showStyleSettings;
            myTarget.showAudioSettings = showAudioSettings;
            myTarget.showUnityEventSettings = showUnityEventSettings;
        }



        void GenerateStyle()
        {
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 1);
            }
        }

        void HorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal)
        {
            float myMaxWidth;
            //not to self: it's ternary operator not tarnary operator. Stop mistyping
            if (settings)
                myMaxWidth = fieldSize == FieldSize.small ? settings.smallHorizontalFieldSize : fieldSize == FieldSize.normal ? settings.normalltHorizontalFieldSize : fieldSize == FieldSize.large ? settings.largeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? settings.extraLargeHorizontalFieldSize : settings.normalltHorizontalFieldSize;
            else
                myMaxWidth = fieldSize == FieldSize.small ? defaultSmallHorizontalFieldSize : fieldSize == FieldSize.normal ? defaultNormalltHorizontalFieldSize : fieldSize == FieldSize.large ? defaultLargeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? defaultExtraLargeHorizontalFieldSize : settings.normalltHorizontalFieldSize;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent(label, tooltip);
            EditorGUILayout.LabelField(gUIContent, GUILayout.MaxWidth(myMaxWidth));
            EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
        }
        //void DrawUILine(Color color, int thickness = 1, int padding = 0)
        //{
        //    Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        //    r.height = thickness;
        //    r.y += padding / 2;
        //    r.x -= 2;
        //    r.width += 6;
        //    EditorGUI.DrawRect(r, color);
        //}
    }
}