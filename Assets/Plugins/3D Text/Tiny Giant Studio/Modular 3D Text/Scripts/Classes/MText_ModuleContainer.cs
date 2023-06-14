using UnityEngine;

namespace MText
{
    [System.Serializable]
    public class MText_ModuleContainer
    {
        public MText_Module module;
        public VariableHolder[] variableHolders;
    }

    [System.Serializable]
    public class VariableHolder
    {
        public string variableName;
        public ModuleVariableType type;
        [HideInInspector]
        public float floatValue;
        [HideInInspector]
        public int intValue;
        [HideInInspector]
        public bool boolValue;
        [HideInInspector]
        public string stringValue;
        [HideInInspector]
        public Vector2 vector2Value;
        [HideInInspector]
        public Vector3 vector3Value;
        [HideInInspector]
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [HideInInspector]
        public GameObject gameObjectValue;
        [HideInInspector]
        public PhysicMaterial physicMaterialValue;
        [HideInInspector]
        public string hideIf;
    }

    [System.Serializable]
    public enum ModuleVariableType
    {
        @float,
        @int,
        @bool,
        @string,
        vector2,
        vector3,
        animationCurve,
        gameObject,
        physicMaterial
    }
}