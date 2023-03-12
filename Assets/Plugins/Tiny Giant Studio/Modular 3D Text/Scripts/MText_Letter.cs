using UnityEditor;
using UnityEngine;

/// <summary>
/// This is used to create a single alphabet
/// 
/// When to use it:
///     While creating a new font you can use this to fit the font to a standard size so that you can swap fonts ingame and avoid unexpected behaviours
///     The models of your font have different sizes and anchor positions/// 
/// 
/// How to use:
///     1. Create an empty game object 
///     2. Add this to it
///     3. Add your model as a child object
///     4. Fit it in the square you see in editor mode
///     5. Save the game object as a prefab
///     6. Select your preferred font, assign the PREFAB to the character of your chosing
/// </summary>

namespace MText
{
    [ExecuteInEditMode]
    public class MText_Letter : MonoBehaviour
    {
        public Renderer model; //this is a reference to the actual model with material to swap when typing

#if UNITY_EDITOR
        readonly float length = 0.13f, height = 0.13f, depth = .03f; //don't change
        readonly float instructionTextHeight = 1;

        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 0, 0, 1f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(length, height, depth));

            // Enable codes below to show a a label with instruction and an arrow. doesnt really do anything            
            GUIStyle myStyle = new GUIStyle
            {
                fontSize = 6,
                alignment = TextAnchor.UpperCenter
            };
            Handles.Label(transform.position + new Vector3(0, instructionTextHeight, 0), "fit here, arrow is forward", myStyle);

            //Handles.ArrowCap(0, transform.position, transform.rotation * Quaternion.Euler(0, 00, 90), 3);
        }
#endif
    }
}