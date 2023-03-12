using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(LinearLayoutGroup))]
    public class LinearLayoutGroupEditor : Editor
    {
        private static GUIStyle foldOutStyle = null;
        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);

        LinearLayoutGroup myTarget;
        SerializedObject soTarget;


        SerializedProperty autoItemSize;
        SerializedProperty bounds;
        SerializedProperty spacing;
        SerializedProperty alignment;



        void OnEnable()
        {
            myTarget = (LinearLayoutGroup)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }



            // Show default inspector property editor
            //DrawDefaultInspector();

            EditorGUILayout.PropertyField(alignment, GUIContent.none);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(autoItemSize);
            EditorGUILayout.PropertyField(spacing);
            EditorGUILayout.Space(5);

            DebugFoldout();
            if (EditorGUI.EndChangeCheck())
            {
                //soTarget.ApplyModifiedProperties();
                LinearLayoutGroup.Alignment anchor = myTarget.alignment;
                if (soTarget.ApplyModifiedProperties())
                {
                    if (myTarget.GetComponent<Modular3DText>())
                    {
                        if (anchor != myTarget.alignment)
                        {
                            myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                        }
                        else
                        {
                            //if (!myTarget.GetComponent<Modular3DText>().ShouldItCreateChild())
                            {
                                myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(myTarget);
            }
        }

        private void DebugFoldout()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            myTarget.showDebugSettings.target = EditorGUILayout.Foldout(myTarget.showDebugSettings.target, "Debug", true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(myTarget.showDebugSettings.faded))
            {
                EditorGUILayout.PropertyField(bounds);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void FindProperties()
        {
            bounds = soTarget.FindProperty("bounds");
            autoItemSize = soTarget.FindProperty("autoItemSize");
            spacing = soTarget.FindProperty("spacing");
            alignment = soTarget.FindProperty("alignment");
        }
    }
}