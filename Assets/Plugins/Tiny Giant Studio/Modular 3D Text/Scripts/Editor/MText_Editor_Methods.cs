using UnityEditor;
using UnityEngine;

namespace MText
{
    public static class MText_Editor_Methods
    {
        //public static MText_Settings settings;

        readonly private static float defaultSmallHorizontalFieldSize = 72.5f;
        readonly private static float defaultNormalltHorizontalFieldSize = 100;
        readonly private static float defaultLargeHorizontalFieldSize = 120f;
        readonly private static float defaultExtraLargeHorizontalFieldSize = 155f;
        readonly private static float defaultGiganticHorizontalFieldSize = 220;


        public static string GetPropertyName(ModuleVariableType type)
        {
            string propertyName;
            if (type == ModuleVariableType.@float)
                propertyName = "floatValue";
            else if (type == ModuleVariableType.@int)
                propertyName = "intValue";
            else if (type == ModuleVariableType.@bool)
                propertyName = "boolValue";
            else if (type == ModuleVariableType.@string)
                propertyName = "stringValue";
            else if (type == ModuleVariableType.vector2)
                propertyName = "vector2Value";
            else if (type == ModuleVariableType.vector3)
                propertyName = "vector3Value";
            else if (type == ModuleVariableType.animationCurve)
                propertyName = "animationCurve";
            else if (type == ModuleVariableType.gameObject)
                propertyName = "gameObjectValue";
            else if (type == ModuleVariableType.physicMaterial)
                propertyName = "physicMaterialValue";
            else
                propertyName = "floatValue";
            return propertyName;
        }

        public static void HorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal)
        {
            if (property == null)
                return;

            float myMaxWidth;

            //if (settings)
            //    myMaxWidth = fieldSize == FieldSize.small ? settings.smallHorizontalFieldSize : fieldSize == FieldSize.normal ? settings.normalltHorizontalFieldSize : fieldSize == FieldSize.large ? settings.largeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? settings.extraLargeHorizontalFieldSize : settings.normalltHorizontalFieldSize;
            //else
            myMaxWidth = fieldSize == FieldSize.small ? defaultSmallHorizontalFieldSize : fieldSize == FieldSize.normal ? defaultNormalltHorizontalFieldSize : fieldSize == FieldSize.large ? defaultLargeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? defaultExtraLargeHorizontalFieldSize: fieldSize == FieldSize.gigantic ? defaultGiganticHorizontalFieldSize : defaultNormalltHorizontalFieldSize;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent(label, tooltip);
            EditorGUILayout.LabelField(gUIContent, GUILayout.MaxWidth(myMaxWidth));
            EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
        }
    }
}