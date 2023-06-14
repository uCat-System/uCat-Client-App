using UnityEditor;
using UnityEngine;

namespace MText
{
    [CustomPropertyDrawer(typeof(MText_Character))]
    public class MText_CharacterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;

            // Calculate rects
            var characterRect = new Rect(position.x, position.y, 40, position.height);
            var spacingRect = new Rect(position.x + 45, position.y, 40, position.height);
            var prefabtRect = new Rect(position.x + 95, position.y, (position.width - 95) / 2, position.height);
            var meshtRect = new Rect(position.x + 95 + ((position.width - 95) / 2), position.y, (position.width - 95) / 2, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(characterRect, property.FindPropertyRelative("character"), GUIContent.none);
            EditorGUI.PropertyField(spacingRect, property.FindPropertyRelative("spacing"), GUIContent.none);
            EditorGUI.PropertyField(prefabtRect, property.FindPropertyRelative("prefab"), GUIContent.none);
            EditorGUI.PropertyField(meshtRect, property.FindPropertyRelative("meshPrefab"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
