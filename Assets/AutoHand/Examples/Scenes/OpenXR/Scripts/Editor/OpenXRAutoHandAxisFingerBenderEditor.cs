using Autohand.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Autohand {
    [CustomEditor(typeof(OpenXRAutoHandAxisFingerBender))]
    public class OpenXRAutoHandAxisFingerBenderEditor : Editor{
        OpenXRAutoHandAxisFingerBender bender;

        void OnEnable() {
            bender = target as OpenXRAutoHandAxisFingerBender;
        }

        public override void OnInspectorGUI() {
            EditorUtility.SetDirty(bender);

            DrawDefaultInspector();
            EditorGUILayout.Space();
            if(bender.hand != null) {
                if(bender.bendOffsets.Length != bender.hand.fingers.Length)
                    bender.bendOffsets = new float[bender.hand.fingers.Length];
                for(int i = 0; i < bender.hand.fingers.Length; i++) {
                    var layout = EditorGUILayout.GetControlRect();
                    layout.width /= 2;
                    var text = new GUIContent(bender.hand.fingers[i].name + " Offset", "0 is no bend, 0.5 is half bend, 1 is full bend, -1 to stiffen finger from sway");
                    EditorGUI.LabelField(layout, text);
                    layout.x += layout.width;
                    bender.bendOffsets[i] = EditorGUI.FloatField(layout, bender.bendOffsets[i]);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
