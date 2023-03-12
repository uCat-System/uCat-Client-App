using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace MText
{
    [CustomEditor(typeof(CircularLayoutGroup))]
    public class CircularLayoutGroupEditor : Editor
    {
        CircularLayoutGroup myTarget;
        SerializedObject soTarget;

        SerializedProperty autoItemSize;
        SerializedProperty direction;
        SerializedProperty style;

        SerializedProperty spread;
        SerializedProperty radius;
        SerializedProperty radiusDecreaseRate;

        SerializedProperty useRotation;
        SerializedProperty rotation;

        void OnEnable()
        {
            myTarget = (CircularLayoutGroup)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(direction, GUIContent.none);

            GUILayout.Space(10);

            Color contentColor = GUI.contentColor;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useRotation, GUIContent.none, GUILayout.MaxWidth(20));

            if (!myTarget.useRotation)
                GUI.contentColor = Color.gray;
            else
                GUI.contentColor = contentColor;

            MText_Editor_Methods.HorizontalField(rotation, "Rotation", "", FieldSize.small);
            //EditorGUILayout.PropertyField(rotation);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (myTarget.useRotation)
                GUI.contentColor = Color.gray;
            else
                GUI.contentColor = contentColor;
            //myTarget.useRotation = EditorGUILayout.Toggle(!myTarget.useRotation, GUILayout.MaxWidth(20));
            EditorGUILayout.PropertyField(style, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            GUI.contentColor = contentColor;

            EditorGUILayout.PropertyField(spread);
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(radiusDecreaseRate);
            EditorGUILayout.PropertyField(autoItemSize);

            DrawAutoItemSize();

            if (EditorGUI.EndChangeCheck())
            {
                if (soTarget.ApplyModifiedProperties())
                {
                    if (myTarget.GetComponent<Modular3DText>())
                        myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                }
                EditorUtility.SetDirty(myTarget);
            }
        }

        private void DrawAutoItemSize()
        {
            if (!myTarget.GetComponent<Modular3DText>())
            {
                EditorGUILayout.PropertyField(autoItemSize);
            }
        }

        void FindProperties()
        {
            useRotation = soTarget.FindProperty("useRotation");
            rotation = soTarget.FindProperty("rotation");

            autoItemSize = soTarget.FindProperty("autoItemSize");
            direction = soTarget.FindProperty("direction");
            style = soTarget.FindProperty("style");

            spread = soTarget.FindProperty("spread");
            radius = soTarget.FindProperty("radius");
            radiusDecreaseRate = soTarget.FindProperty("radiusDecreaseRate");
        }
    }
}