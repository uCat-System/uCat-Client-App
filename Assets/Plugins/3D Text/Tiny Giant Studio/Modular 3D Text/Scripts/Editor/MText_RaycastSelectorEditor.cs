using MText;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


[CustomEditor(typeof(MText_UI_RaycastSelector))]
public class MText_RaycastSelectorEditor : Editor
{
    MText_UI_RaycastSelector myTarget;

    void OnEnable()
    {
        myTarget = (MText_UI_RaycastSelector)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!myTarget.GetComponent<MText_UI_RaycastSelectorInputProcessor>())
        {
            EditorGUILayout.HelpBox("No input processor found. If you are using a custom solution, ignore this warning. Otherwise, please add Raycast Selector Input Processor", MessageType.Info);
            if (GUILayout.Button("Add Raycast Selector Input Processor"))
            {
                myTarget.gameObject.AddComponent<MText_UI_RaycastSelectorInputProcessor>();
                EditorUtility.SetDirty(myTarget.gameObject);
            }
        }
    }
}
