using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(MText_Module), true)]
    public class MText_ModuleEditor : Editor
    {
        MText_Module myTarget;
        SerializedObject soTarget;

        SerializedProperty variableHolders;

        void OnEnable()
        {
            myTarget = (MText_Module)target;
            soTarget = new SerializedObject(target);

            variableHolders = soTarget.FindProperty("variableHolders");
        }

        Vector2 scrollPos;
        public override void OnInspectorGUI()
        {
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            if (myTarget.variableHolders != null)
            {
                EditorGUILayout.HelpBox("Modifiable variables by text. These are dependent on individual modules. Changing them might result in undesirable effects. If that happens, please fix them from the instruction in code summary or download the original version from the asset store.", MessageType.Warning);

                int indexWidth = 16;
                int nameWidth = 90;
                int typeMinWidth = 70;
                int typeMaxWidth = 125;
                int defaultValueWidth = 70;
                int hideIfWidth = 40;

                scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);

                if (myTarget.variableHolders.Length > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", EditorStyles.miniLabel, GUILayout.MaxWidth(indexWidth));
                    EditorGUILayout.LabelField("Name", EditorStyles.centeredGreyMiniLabel, GUILayout.MinWidth(nameWidth));
                    EditorGUILayout.LabelField("Type", EditorStyles.centeredGreyMiniLabel, GUILayout.MinWidth(typeMinWidth), GUILayout.MaxWidth(typeMaxWidth));
                    EditorGUILayout.LabelField(new GUIContent("Hide if", "The variable will be invisible if the bool variable with show if name is set to true."), EditorStyles.centeredGreyMiniLabel, GUILayout.MinWidth(hideIfWidth));
                    EditorGUILayout.LabelField(new GUIContent("defaultValue"), EditorStyles.centeredGreyMiniLabel, GUILayout.MinWidth(defaultValueWidth));
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(20), GUILayout.MinWidth(20));
                    EditorGUILayout.EndHorizontal();
                }

                for (int i = 0; i < myTarget.variableHolders.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    Color color = GUI.color;
                    GUI.color = Color.gray;
                    EditorGUILayout.LabelField("" + i, GUILayout.MaxWidth(indexWidth)); //Index
                    GUI.color = color;

                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("variableName"), GUIContent.none, GUILayout.MinWidth(nameWidth));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("type"), GUIContent.none, GUILayout.MinWidth(typeMinWidth), GUILayout.MaxWidth(typeMaxWidth));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("hideIf"), GUIContent.none, GUILayout.MinWidth(hideIfWidth));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative(MText_Editor_Methods.GetPropertyName(myTarget.variableHolders[i].type)), GUIContent.none, GUILayout.MinWidth(defaultValueWidth));
                    //if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                    GUI.color = Color.yellow;
                    //GUI.color = new Color(1f,127/255f,39/255f,1f);
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                    {
                        RemoveVariable(i);
                    }
                    GUI.color = color;
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add new variable"))
                {
                    System.Array.Resize(ref myTarget.variableHolders, myTarget.variableHolders.Length + 1);
                    EditorUtility.SetDirty(myTarget);
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                myTarget.variableHolders = new VariableHolder[0];
            }

            DrawDefaultInspector();


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
            }
        }

        void RemoveVariable(int removeTarget)
        {
            for (int i = removeTarget; i < myTarget.variableHolders.Length - 1; i++)
            {
                myTarget.variableHolders[i] = myTarget.variableHolders[i + 1];
            }
            System.Array.Resize(ref myTarget.variableHolders, myTarget.variableHolders.Length - 1);
            EditorUtility.SetDirty(myTarget);
        }
    }
}