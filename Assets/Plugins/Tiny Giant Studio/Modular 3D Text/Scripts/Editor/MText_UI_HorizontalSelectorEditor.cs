using UnityEditor;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(MText_UI_HorizontalSelector))]
    public class MText_UI_HorizontalSelectorEditor : Editor
    {
        public MText_Settings settings;

        MText_UI_HorizontalSelector myTarget;
        SerializedObject soTarget;

        SerializedProperty text;
        SerializedProperty selected;
        SerializedProperty interactable;

        SerializedProperty options;
        SerializedProperty value;



        SerializedProperty audioSource;
        SerializedProperty valueChangeSoundEffect;

        SerializedProperty onSelectEvent;
        SerializedProperty onValueChangedEvent;
        SerializedProperty onValueIncreasedEvent;
        SerializedProperty onValueDecreasedEvent;



        bool showAudioControls;
        bool showEventsControls;



        GUIStyle foldOutStyle;
        Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);


        void OnEnable()
        {
            myTarget = (MText_UI_HorizontalSelector)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }

        public override void OnInspectorGUI()
        {
            GenerateStyle();

            if (myTarget.text == null)
                EditorGUILayout.HelpBox("Please reference a text", MessageType.Warning);

            EditorGUI.BeginChangeCheck();

            //DrawDefaultInspector();

            MText_Editor_Methods.HorizontalField(text, "Text", "Reference to the 3d text where the selected text will be shown");
            GUILayout.Space(10);
            MText_Editor_Methods.HorizontalField(selected, "Selected", "If keyboard control is enabled, selected = you can control via selected. \nThis value will be controlled by list, if it is in one");
            MText_Editor_Methods.HorizontalField(interactable, "Interactable", "If keyboard control is enabled, selected = you can control via selected\nOr selected/deselected in a List");
            GUILayout.Space(15);
            //MText_Editor_Methods.HorizontalField(value, "Current Value");
            EditorGUILayout.PropertyField(value);
            EditorGUILayout.PropertyField(options);
            GUILayout.Space(15);

            AudioControl();
            EventControl();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                myTarget.UpdateText();
            }
        }




        private void AudioControl()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showAudioControls = EditorGUILayout.Foldout(showAudioControls, "Audio", true, foldOutStyle);
            GUILayout.EndVertical();

            if (showAudioControls)
            {
                EditorGUI.indentLevel = 1;
                MText_Editor_Methods.HorizontalField(audioSource, "Audio Source");
                MText_Editor_Methods.HorizontalField(valueChangeSoundEffect, "Value Changed SFX");

            }
            if (!Selection.activeTransform)
            {
                showAudioControls = false;
            }
            GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }
        private void EventControl()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showEventsControls = EditorGUILayout.Foldout(showEventsControls, "Events", true, foldOutStyle);
            GUILayout.EndVertical();

            if (showEventsControls)
            {
                EditorGUI.indentLevel = 0;
                EditorGUILayout.PropertyField(onSelectEvent);
                EditorGUILayout.PropertyField(onValueChangedEvent);
                EditorGUILayout.PropertyField(onValueIncreasedEvent);
                EditorGUILayout.PropertyField(onValueDecreasedEvent);
            }
            if (!Selection.activeTransform)
            {
                showEventsControls = false;
            }
            GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }



        private void FindProperties()
        {
            text = soTarget.FindProperty("text");
            selected = soTarget.FindProperty("selected");
            interactable = soTarget.FindProperty("interactable");

            options = soTarget.FindProperty("options");

            value = soTarget.FindProperty("value");

            audioSource = soTarget.FindProperty("audioSource");
            valueChangeSoundEffect = soTarget.FindProperty("valueChangeSoundEffect");

            onSelectEvent = soTarget.FindProperty("onSelectEvent");
            onValueChangedEvent = soTarget.FindProperty("onValueChangedEvent");
            onValueIncreasedEvent = soTarget.FindProperty("onValueIncreasedEvent");
            onValueDecreasedEvent = soTarget.FindProperty("onValueDecreasedEvent");
        }

        private void GenerateStyle()
        {
            if (foldOutStyle == null)
            {
                //foldOutStyle = new GUIStyle(EditorStyles.foldout);
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    //foldOutStyle.overflow = new RectOffset(-10, 0, 3, 0);
                    padding = new RectOffset(14, 0, 0, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }
        }
    }
}