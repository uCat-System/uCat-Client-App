using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace MText
{
    //[CreateAssetMenu(menuName = "Modular 3d Text/Settings")]
    public class MText_Settings : ScriptableObject
    {
        [HideInInspector] public string selectedTab = "Getting Started";

        public Color tabSelectedColor = Color.white;
        public Color tabUnselectedColor = new Color(0.9f, 0.9f, 0.9f);
        [Space]
        public Color gridItemColor = new Color(0.9f, 0.9f, 0.9f);


        //Text
        public MText_Font defaultFont = null;
        public Vector3 defaultTextSize = new Vector3(8, 8, 2);
        public Material defaultTextMaterial = null;

        //Button
        public Vector3 defaultButtonNormalTextSize = new Vector3(8, 8, 2);
        public Material defaultButtonNormalTextMaterial = null;
        public Material defaultButtonNormalBackgroundMaterial = null;

        public Vector3 defaultButtonSelectedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultButtonSelectedTextMaterial = null;
        public Material defaultButtonSelectedBackgroundMaterial = null;

        public Vector3 defaultButtonPressedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultButtonPressedTextMaterial = null;
        public Material defaultButtonPressedBackgroundMaterial = null;

        public Vector3 defaultButtonDisabledTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultButtonDisabledTextMaterial = null;
        public Material defaultButtonDisabledBackgroundMaterial = null;

        //List
        public Vector3 defaultListNormalTextSize = new Vector3(8, 8, 2);
        public Material defaultListNormalTextMaterial = null;
        public Material defaultListNormalBackgroundMaterial = null;

        public Vector3 defaultListSelectedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultListSelectedTextMaterial = null;
        public Material defaultListSelectedBackgroundMaterial = null;

        public Vector3 defaultListPressedTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultListPressedTextMaterial = null;
        public Material defaultListPressedBackgroundMaterial = null;

        public Vector3 defaultListDisabledTextSize = new Vector3(8.2f, 8.2f, 5);
        public Material defaultListDisabledTextMaterial = null;
        public Material defaultListDisabledBackgroundMaterial = null;


        //[HideInInspector] public bool createLogTextFile = false;
        //[HideInInspector] public bool createConsoleLogs = false;

        [Header("Inspector field size")]
        public float smallHorizontalFieldSize = 72.5f;
        public float normalltHorizontalFieldSize = 100;
        public float largeHorizontalFieldSize = 132.5f;
        public float extraLargeHorizontalFieldSize = 150f;

        public enum MeshExportStyle
        {
            exportAsObj,
            exportAsMeshAsset
        }



#region Font creation
        //font creation settings
        public enum CharInputStyle
        {
            CharacterRange,
            UnicodeRange,
            CustomCharacters,
            UnicodeSequence
            //CharacterSet
        }

        public CharInputStyle charInputStyle;

        public char startChar = '!'; //default '!'
        public char endChar = '~'; //default '~'
        public string startUnicode = "0021"; //default
        [HideInInspector] public string endUnicode = "007E"; //default 

        [HideInInspector]
        [TextArea(10, 99)]
        public string customCharacters = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"; //default 

        [HideInInspector]
        [TextArea(10, 99)]
        public string unicodeSequence = "\\u0021-\\u007E"; //default     


        [HideInInspector] public int vertexDensity = 1; //default 1
        [HideInInspector] public float sizeXY = 1; //default 1
        [HideInInspector] public float sizeZ = 1; //default 1
        [HideInInspector] public float smoothingAngle = 30; //default 30

        [HideInInspector] public MeshExportStyle meshExportStyle = MeshExportStyle.exportAsObj;


#if ENABLE_INPUT_SYSTEM
        public InputActionAsset inputActionAsset;
#endif


        public void ResetFontCreationSettings()
        {
            charInputStyle = CharInputStyle.CharacterRange;
            startChar = '!';
            endChar = '~';
            startUnicode = "0021";
            endUnicode = "007E";
            customCharacters = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            unicodeSequence = "\\u0021-\\u007E";


            vertexDensity = 1;
            sizeXY = 1;
            sizeZ = 1;
            smoothingAngle = 30;

            meshExportStyle = MeshExportStyle.exportAsObj;
        }
#endregion





    }
}
