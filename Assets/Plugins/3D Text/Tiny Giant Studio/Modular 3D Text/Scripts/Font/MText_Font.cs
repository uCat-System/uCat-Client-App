/// Created by Ferdowsur Asif @ Tiny Giant Studio
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor.AnimatedValues;
#endif

namespace MText
{
    [PreferBinarySerialization]
    [CreateAssetMenu(fileName = "New 3D Font", menuName = "Modular 3d Text/Font/New Font")]
    public class MText_Font : ScriptableObject
    {
        public float unitPerEM = 1;

        public List<MText_Character> characters = new List<MText_Character>();

        [Tooltip("The 3d object with the characters as child object. NOT required.")]
        [FormerlySerializedAs("source")]
        public GameObject modelSource = null;

        [Tooltip("Use UpperCase If LowerCase Is Missing")]
        public bool useUpperCaseLettersIfLowerCaseIsMissing = true;


        [Tooltip("Monospace means all characters are spaced equally.\nIf turned on, individual spacing value from list below is ignored. The information is not removed to avoid accidentally turning it on ruin the font. \nCharacter spacing is used for everything")]
        public bool monoSpaceFont;
        public float monoSpaceSpacing = 1;
        [Tooltip("Word spacing and spacing for unavailable characters")]
        public float emptySpaceSpacing = 1;
        [Tooltip("Text's character spacing = font's character spacing * text's character spacing")]
        public float characterSpacing = 1;
        public float lineHeight = 0.1469311f;





        [Space]
        [Tooltip("Avoid recursive references")]
        public List<MText_Font> fallbackFonts = new List<MText_Font>();




        public bool enableKerning = true;
        public float kerningMultiplier = 1f;
        //unfortunately dictionary isn't serializable //TODO
        public List<MText_KernPairHolder> kernTable = new List<MText_KernPairHolder>();

        public float TabSpace() => emptySpaceSpacing * 3;



        #region Font file data
        [SerializeField] //It's not serializable though
        private FontCreation.MText_CharacterGenerator.TypeFace _typeFace;
        public FontCreation.MText_CharacterGenerator.TypeFace TypeFace
        {
            get
            {
                if (_typeFace == null)
                    GetTypeFaceFromBytes();

                return _typeFace;
            }
            set
            {
                _typeFace = value;
            }
        }

        public byte[] fontBytes;


        public void SetFontBytes(byte[] rfontBytes)
        {
            fontBytes = rfontBytes;
            GetTypeFaceFromBytes();
        }

        public void GetTypeFaceFromBytes()
        {
            _typeFace = new FontCreation.MText_CharacterGenerator.TypeFace(fontBytes);
        }

        public int sizeXYInput = 1;
        public int sizeZInput = 1;
        public float vertexDensityInput = 1;
        public float autoSmoothAngleInput = 30;
        public float averageYValue = 0;
        #endregion




#if UNITY_EDITOR
        /// <summary>
        /// Editor only. Used by inspector.
        /// </summary>
        public bool showCharacterDetailsEditor;

        /// <summary>
        /// Editor only. Used by inspector.
        /// </summary>
        public string beingSearched;
#endif

        /// <summary>
        /// When this returns null, MText_CharacterCreator is used to create it on the fly from ttf data
        /// </summary>
        /// <param name="c"></param>
        /// <param name="checkFallBackFonts"></param>
        /// <returns></returns>
        public (Mesh, MText_Character) RetrievePrefabAndCharacter(char c, bool checkFallBackFonts = true)
        {
            //look for character in any form
            for (int i = 0; i < characters.Count; i++)
            {
                if (c == characters[i].character)
                {
                    return (MeshPrefab(i), characters[i]);
                }
            }

            //if no character is found of lower case
            if (useUpperCaseLettersIfLowerCaseIsMissing)
            {
                if (char.IsLower(c))
                {
                    c = char.ToUpper(c);

                    for (int i = 0; i < characters.Count; i++)
                    {
                        if (c == characters[i].character)
                        {
                            return (MeshPrefab(i), characters[i]);
                        }
                    }
                }
            }

            if (checkFallBackFonts)
            {
                for (int i = 0; i < fallbackFonts.Count; i++)
                {
                    if (fallbackFonts[i] != null)
                    {
                        var missingCharacter = fallbackFonts[i].RetrievePrefabAndCharacter(c, false);
                        if (missingCharacter.Item1 != null)
                        {
                            return (missingCharacter.Item1, missingCharacter.Item2);
                        }
                    }
                }
            }

            return (null, null);
        }

        Mesh MeshPrefab(int i)
        {
            if (characters[i].prefab)
            {
                if (characters[i].prefab.GetComponent<MeshFilter>())
                {
                    if (characters[i].prefab.GetComponent<MeshFilter>().sharedMesh)
                    {
                        return characters[i].prefab.GetComponent<MeshFilter>().sharedMesh;
                    }
                }
            }
            else if (characters[i].meshPrefab)
            {
                return characters[i].meshPrefab;
            }


            return null;
        }

        public float Spacing(char c)
        {
            if (!monoSpaceFont)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (c == characters[i].character)
                    {
                        return Spacing(characters[i].spacing);
                    }
                }

                for (int i = 0; i < fallbackFonts.Count; i++)
                {
                    if (fallbackFonts[i] != null)
                    {
                        return fallbackFonts[i].Spacing(c);
                    }
                }

                return MonoSpaceSpacing();
            }
            else //for monospace fonts
            {
                return MonoSpaceSpacing();
            }
        }

        /// <summary>
        /// Spacing with kerning
        /// </summary>
        /// <param name="previousCharacter"></param>
        /// <param name="currentCharacter"></param>
        /// <returns></returns>
        public float Spacing(char previousCharacter, char currentCharacter)
        {
            if (!monoSpaceFont)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (currentCharacter == characters[i].character)
                    {
                        //Debug.Log(Kerning(previousCharacter, characters[i]));
                        return Spacing(characters[i].spacing) + Kerning(previousCharacter, characters[i]);
                    }
                }

                for (int i = 0; i < fallbackFonts.Count; i++)
                {
                    if (fallbackFonts[i] != null)
                    {
                        return fallbackFonts[i].Spacing(currentCharacter);
                    }
                }

                return MonoSpaceSpacing();
            }
            else //for monospace fonts
            {
                return MonoSpaceSpacing();
            }
        }
        /// <summary>
        /// This is the raw value used by unity after taking font EM into consideration
        /// </summary>
        /// <returns></returns>
        public float MonoSpaceSpacing() => ConvertedValue(monoSpaceSpacing);



        float Kerning(char previousChar, MText_Character currentChar)
        {
            MText_Character previousCharacter = Character(previousChar);

            if (previousCharacter == null || currentChar == null)
                return 0;

            MText_KernPair kernPair = new MText_KernPair
            {
                left = previousCharacter.glyphIndex,
                right = currentChar.glyphIndex
            };

            for (int i = 0; i < kernTable.Count; i++)
            {
                if (kernTable[i].kernPair.left == kernPair.left && kernTable[i].kernPair.right == kernPair.right)
                {
                    return ConvertedValue(kernTable[i].offset * kerningMultiplier);
                }
            }

            return 0;
        }

        MText_Character Character(char c)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].character == c)
                {
                    return characters[i];
                }
            }
            return null;
        }


        /// <summary>
        /// Used by getcharacterobject.cs
        /// </summary>
        /// <param name="rawAdvance"></param>
        /// <returns></returns>
        public float Spacing(float rawAdvance)
        {
            return ConvertedValue(rawAdvance);
        }

        public float ConvertedValue(float spacing) => (spacing * characterSpacing) / (Mathf.Clamp(unitPerEM, 0.0001f, 1000000) * 8);


        //Font creation:
        #region Update Character List start
        public void UpdateCharacterList(GameObject prefab)
        {
            modelSource = prefab;
            UpdateCharacterList();
        }

        public void UpdateCharacterList(bool overwriteOldSet)
        {
            if (overwriteOldSet)
                characters.Clear();

            UpdateCharacterList();
        }

        public void UpdateCharacterList()
        {
            if (modelSource)
            {
                foreach (Transform child in modelSource.transform)
                {
                    AddCharacter(child.gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Model source not found on " + name + " couldn't add any characters");
            }

            //TabSpace = emptySpaceSpacing * 3;
        }

        public void AddCharacter(GameObject obj)
        {
            MText_Character newChar = new MText_Character();

            if (!obj)
                return;

            ProcessName(obj.name, out char character, out float spacing);

            newChar.character = character;
            newChar.spacing = spacing;
            newChar.prefab = obj;

            characters.Add(newChar);
        }
        public void AddCharacter(Mesh mesh)
        {
            MText_Character newChar = new MText_Character();

            if (!mesh)
                return;

            ProcessName(mesh.name, out char character, out float spacing);

            newChar.character = character;
            newChar.spacing = spacing;


            newChar.meshPrefab = mesh;

            characters.Add(newChar);
        }


        private void ProcessName(string name, out char character, out float spacing)
        {
            try
            {
                NewMethod(name, out character, out spacing);
            }
            catch
            {
                //Debug.Log("Old method");
                OldMethod(name, out character, out spacing);
            }
        }
        private void NewMethod(string name, out char character, out float spacing)
        {
            string[] s = name.Split(new char[] { '_', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log(s[0]);
            character = Regex.Unescape("\\u" + s[0]).ToCharArray()[0];
            //Debug.Log(s[1]);
            spacing = GetSpacing(s[1]);
        }
        private void OldMethod(string name, out char character, out float spacing)
        {
            if (name.Contains("dot"))
            {
                character = '.';
                spacing = (float)Convert.ToDouble(name.Substring(4));

            }
            else if (name.Contains("forwardSlash"))
            {
                character = '/';
                //spacing = (float)Convert.ToDouble(name.Substring(13));
                spacing = GetSpacing(name.Substring(13));
            }
            else if (name.Contains("quotationMark"))
            {
                character = '"';
                //spacing = (float)Convert.ToDouble(name.Substring(14));
                spacing = GetSpacing(name.Substring(14));
            }
            else if (name.Contains("multiply"))
            {
                character = '*';
                //spacing = (float)Convert.ToDouble(name.Substring(9));
                spacing = GetSpacing(name.Substring(9));
            }
            else if (name.Contains("colon"))
            {
                character = ':';
                //spacing = (float)Convert.ToDouble(name.Substring(6));
                spacing = GetSpacing(name.Substring(6));
            }
            else if (name.Contains("lessThan"))
            {
                character = '<';
                //spacing = (float)Convert.ToDouble(name.Substring(9));
                spacing = GetSpacing(name.Substring(9));
            }
            else if (name.Contains("moreThan"))
            {
                character = '>';
                //spacing = (float)Convert.ToDouble(name.Substring(9));
                spacing = GetSpacing(name.Substring(9));
            }
            else if (name.Contains("questionMark"))
            {
                character = '?';
                //spacing = (float)Convert.ToDouble(name.Substring(13));
                spacing = GetSpacing(name.Substring(13));
            }
            else if (name.Contains("slash"))
            {
                character = '/';
                //spacing = (float)Convert.ToDouble(name.Substring(6));
                spacing = GetSpacing(name.Substring(6));
            }
            else if (name.Contains("backwardSlash"))
            {
                character = '\\';
                //spacing = (float)Convert.ToDouble(name.Substring(14));
                spacing = GetSpacing(name.Substring(14));
            }
            else if (name.Contains("verticalLine"))
            {
                character = '|';
                //spacing = (float)Convert.ToDouble(name.Substring(13));
                spacing = GetSpacing(name.Substring(13));
            }
            else
            {
                char[] chars = name.ToCharArray();
                character = chars[0];
                //spacing = (float)Convert.ToDouble(name.Substring(2));
                spacing = GetSpacing(name.Substring(2));
            }
            spacing *= 0.81f;
        }
        private float GetSpacing(string numberString)
        {
            if (float.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                return value;
            else
                return 1;
            //return (float)Convert.ToDouble(numberString);
        }

        [ContextMenu("GetMonoSpacingFromAverageCharacterSpacing")]
        public void GetMonoSpacingFromAverageCharacterSpacing()
        {
            float spacing = 0;
            for (int i = 0; i < characters.Count; i++)
            {
                spacing += characters[i].spacing;
            }
            monoSpaceSpacing = spacing / characters.Count;
        }
        #endregion Update Character List end
    }
}