using UnityEditor;
/// Created by Ferdowsur Asif @ Tiny Giant Studio
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(MText_UI_Toggle))]
    public class MText_UI_ToggleEditor : Editor
    {
        MText_UI_Toggle myTarget;
        SerializedObject soTarget;

        SerializedProperty isOn;
        SerializedProperty activeGraphic;
        SerializedProperty inactiveGraphic;


        void OnEnable()
        {
            myTarget = (MText_UI_Toggle)target;
            soTarget = new SerializedObject(target);

            isOn = soTarget.FindProperty("_isOn");
            activeGraphic = soTarget.FindProperty("activeGraphic");
            inactiveGraphic = soTarget.FindProperty("inactiveGraphic");
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            MainSettings();

            if (EditorGUI.EndChangeCheck())
            {
                myTarget.VisualUpdate();
                soTarget.ApplyModifiedProperties();
                ApplyMyModifiedData();
                EditorUtility.SetDirty(myTarget);
            }
        }

        void ApplyMyModifiedData()
        {
            if (myTarget.IsOn)
                myTarget.ActiveVisualUpdate();
            else
                myTarget.InactiveVisualUpdate();
        }

        void MainSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;
            GUILayout.Space(2.5f);
            EditorGUILayout.PropertyField(isOn);
            DrawUILine(Color.grey, 1, 2);
            EditorGUILayout.PropertyField(activeGraphic, true);
            EditorGUILayout.PropertyField(inactiveGraphic, true);

            GUILayout.EndVertical();
        }

        void DrawUILine(Color color, int thickness = 1, int padding = 0)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}