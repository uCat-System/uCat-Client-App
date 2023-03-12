using UnityEngine;
using MText.FontCreation;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MText
{
    public class MText_GetCharacterObject
    {
        private static readonly float unitConverted = 0.1f; //an arbitary value



        /// <summary>
        /// Used by text to get character mesh + layout info when single mesh is turned on.
        /// The other way to get character is using GetObject(char c, Modular3DText text); for multiple objects
        /// </summary>
        /// <param name="c">The character</param>
        /// <param name="text">The Modular3DText</param>
        /// <returns></returns>
        public static MeshLayout GetMeshLayout(char c, Modular3DText text)
        {
            if (!IsSpecialSymbol(c))
                return ProcessNormalCharacter(c, text);
            else
                return ProcessSpecialCharacter(c, text);
        }

        /// <summary>
        /// When single mesh is turned off
        /// </summary>
        /// <param name="c"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameObject GetObject(char c, Modular3DText text)
        {
            MText_Font font = text.Font;

            GameObject obj = new GameObject();
            LayoutElement layoutElement = obj.AddComponent<LayoutElement>();


            if (!IsSpecialSymbol(c))
                obj = ProcessNormalCharacter(c, text, obj, layoutElement);
            else
                obj = ProcessSpecialCharacter(c, font, text, layoutElement, obj);

            obj.SetActive(false);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckLeftOver(obj, text);

                obj.AddComponent<DelayCallCleanUp>().text = text;
            }
            else
            {
                if (text.hideLettersInHierarchyInPlayMode)
                    obj.hideFlags = HideFlags.HideInHierarchy;
            }

            text._allcharacterObjectList.Add(obj);

            EditorUtility.SetDirty(obj); //without this, unity won't save the character
#endif


            return obj;
        }

        /// <summary>
        /// When single mesh is turned off
        /// </summary>
        /// <param name="c"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GameObject GetObject(char previousChar, char currentChar, Modular3DText text)
        {
            MText_Font font = text.Font;

            GameObject obj = new GameObject();
            LayoutElement layoutElement = obj.AddComponent<LayoutElement>();


            if (!IsSpecialSymbol(currentChar))
                obj = ProcessNormalCharacter(previousChar, currentChar, text, obj, layoutElement);
            else
                obj = ProcessSpecialCharacter(currentChar, font, text, layoutElement, obj); //todo previous char

            obj.SetActive(false);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckLeftOver(obj, text);

                obj.AddComponent<DelayCallCleanUp>().text = text;
            }
            else
            {
                if (text.hideLettersInHierarchyInPlayMode)
                    obj.hideFlags = HideFlags.HideInHierarchy;
            }

            text._allcharacterObjectList.Add(obj);

            EditorUtility.SetDirty(obj); //without this, unity won't save the character
#endif


            return obj;
        }

        private static MeshLayout ProcessNormalCharacter(char c, Modular3DText text)
        {
            bool AutoLetterSize = text.AutoLetterSize;
            MeshLayout meshLayout = new MeshLayout();

            //returns mesh and mtext_Character
            var meshPrefab = text.Font.RetrievePrefabAndCharacter(c);

            bool useAutoSizeAnyway = false;
            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                meshLayout.mesh = meshPrefab.Item1;

                if (!AutoLetterSize)
                {
                    if (meshPrefab.Item2 != null) //TODO: why it can be nulled
                    {
                        meshLayout.xOffset = meshPrefab.Item2.xOffset;
                        meshLayout.yOffset = meshPrefab.Item2.yOffset;
                        meshLayout.zOffset = meshPrefab.Item2.zOffset;
                    }

                    meshLayout.width = text.Font.Spacing(c) * text.FontSize.x;
                    meshLayout.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from ttf data
            else
            {
                MText_CharacterGenerator creator = new MText_CharacterGenerator();
                meshLayout.mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, c);

                if (!AutoLetterSize)
                {
                    if (!text.Font.monoSpaceFont)
                    {
                        if (creator.GylphExists(c))
                            meshLayout.width = text.Font.Spacing(creator.GetChracterAdvance(creator.Index(c))) * text.FontSize.x;
                        else
                            useAutoSizeAnyway = true;
                    }
                    else
                        meshLayout.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    //meshLayout.width = text.Font.Spacing(creator.GetChracterAdvance(creator.Index(c))) * text.FontSize.x;
                    meshLayout.height = text.FontSize.y * unitConverted;
                }
            }


            if (AutoLetterSize || useAutoSizeAnyway)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(meshLayout.mesh);

                meshLayout.xOffset = bounds.center.x;
                meshLayout.yOffset = bounds.center.y;
                meshLayout.zOffset = bounds.center.z;

                meshLayout.width = bounds.size.x * text.FontSize.x;
                meshLayout.height = text.FontSize.y * unitConverted;
            }


            return meshLayout;
        }
        private static GameObject ProcessNormalCharacter(char c, Modular3DText text, GameObject obj, LayoutElement layoutElement)
        {
            bool AutoLetterSize = text.AutoLetterSize;
            obj.name = c.ToString();

            var meshPrefab = text.Font.RetrievePrefabAndCharacter(c);
            obj.AddComponent<MeshFilter>();

            bool useAutoSizeAnyway = false;
            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                obj.GetComponent<MeshFilter>().sharedMesh = meshPrefab.Item1;

#if UNITY_EDITOR
                if (Application.isPlaying)
                    EditorApplication.delayCall += () => SetParent(text, obj); //why delay
#endif
                obj.SetActive(false);

                if (!AutoLetterSize)
                {
                    layoutElement.xOffset = meshPrefab.Item2.xOffset;
                    layoutElement.yOffset = meshPrefab.Item2.yOffset;
                    layoutElement.zOffset = meshPrefab.Item2.zOffset;

                    layoutElement.width = text.Font.Spacing(c) * text.FontSize.x;
                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from ttf data
            else
            {
                MText_CharacterGenerator creator = new MText_CharacterGenerator();
                Mesh mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, c);
                obj.GetComponent<MeshFilter>().sharedMesh = mesh;


                if (!AutoLetterSize)
                {
                    if (!text.Font.monoSpaceFont)
                    {
                        if (creator.GylphExists(c))
                            layoutElement.width = text.Font.Spacing(creator.GetChracterAdvance(creator.Index(c))) * text.FontSize.x;
                        else
                            useAutoSizeAnyway = true;
                    }
                    else
                        layoutElement.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }

            if (AutoLetterSize || useAutoSizeAnyway)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(obj.GetComponent<MeshFilter>().sharedMesh);

                layoutElement.xOffset = bounds.center.x;
                layoutElement.yOffset = bounds.center.y;
                layoutElement.zOffset = bounds.center.z;

                layoutElement.width = bounds.size.x * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
                layoutElement.autoCalculateSize = true;
            }

            return obj;
        }
        private static GameObject ProcessNormalCharacter(char previousChar, char currentChar, Modular3DText text, GameObject obj, LayoutElement layoutElement)
        {
            bool AutoLetterSize = text.AutoLetterSize;
            obj.name = currentChar.ToString();

            var meshPrefab = text.Font.RetrievePrefabAndCharacter(currentChar);
            obj.AddComponent<MeshFilter>();

            //prebuilt character in the 3d font file
            if (meshPrefab.Item1)
            {
                obj.GetComponent<MeshFilter>().sharedMesh = meshPrefab.Item1;

#if UNITY_EDITOR
                if (Application.isPlaying)
                    EditorApplication.delayCall += () => SetParent(text, obj); //why delay
#endif
                obj.SetActive(false);

                if (!AutoLetterSize)
                {
                    layoutElement.xOffset = meshPrefab.Item2.xOffset;
                    layoutElement.yOffset = meshPrefab.Item2.yOffset;
                    layoutElement.zOffset = meshPrefab.Item2.zOffset;

                    layoutElement.width = text.Font.Spacing(previousChar, currentChar) * text.FontSize.x;
                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }
            //Get mesh from ttf data
            else
            {
                MText_CharacterGenerator creator = new MText_CharacterGenerator();
                Mesh mesh = creator.GetMesh(text.Font.GetInstanceID(), text.Font.TypeFace, text.Font.sizeXYInput, text.Font.sizeZInput, text.Font.autoSmoothAngleInput, text.Font.averageYValue, currentChar);
                obj.GetComponent<MeshFilter>().sharedMesh = mesh;


                if (!AutoLetterSize)
                {
                    if (!text.Font.monoSpaceFont)
                        layoutElement.width = text.Font.Spacing(creator.GetChracterAdvance(creator.Index(currentChar))) * text.FontSize.x;
                    else
                        layoutElement.width = text.Font.MonoSpaceSpacing() * text.FontSize.x;

                    layoutElement.height = text.FontSize.y * unitConverted;
                }
            }

            if (AutoLetterSize)
            {
                Bounds bounds = MeshBaseSize.CheckMeshSize(obj.GetComponent<MeshFilter>().sharedMesh);

                layoutElement.xOffset = bounds.center.x;
                layoutElement.yOffset = bounds.center.y;
                layoutElement.zOffset = bounds.center.z;

                layoutElement.width = bounds.size.x * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
                layoutElement.autoCalculateSize = true;
            }

            return obj;
        }

        private static void SetParent(Modular3DText text, GameObject obj)
        {
            if (text)
            {
                if (obj)
                    obj.transform.SetParent(text.transform);
            }
#if UNITY_EDITOR
            else
                Debug.Log(obj, obj);
#endif
        }

        private static bool IsSpecialSymbol(char c)
        {
            if (c == '\t')
                return true;
            if (c == '\n')
                return true;
            if (char.IsWhiteSpace(c))
                return true;
            return false;
        }



        private static MeshLayout ProcessSpecialCharacter(char c, Modular3DText text)
        {
            MeshLayout layoutElement = new MeshLayout();

            if (c == '\n')
            {
                layoutElement.lineBreak = true;
            }
            else if (c == '\t')
            {
                layoutElement.width = text.Font.ConvertedValue(text.Font.TabSpace()) * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }
            else if (char.IsWhiteSpace(c))
            {
                layoutElement.width = text.Font.ConvertedValue(text.Font.emptySpaceSpacing) * text.WordSpacing * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }
            return layoutElement;
        }
        private static GameObject ProcessSpecialCharacter(char c, MText_Font font, Modular3DText text, LayoutElement layoutElement, GameObject obj)
        {
            if (c == '\n')
            {
                obj.name = "New Line";
                layoutElement.lineBreak = true;
            }
            else if (c == '\t')
            {
                obj.name = "Tab";
                layoutElement.width = text.Font.ConvertedValue(font.TabSpace()) * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }
            else if (char.IsWhiteSpace(c))
            {
                obj.name = "Space";
                layoutElement.width = text.Font.ConvertedValue(text.Font.emptySpaceSpacing) * text.WordSpacing * text.FontSize.x;
                layoutElement.height = text.FontSize.y * unitConverted;
            }

            return obj;
        }



#if UNITY_EDITOR
        static void CheckLeftOver(GameObject obj, Modular3DText text)
        {
            if (text)
                return;

            obj.hideFlags = HideFlags.None;

            EditorApplication.delayCall += () =>
            {
                try { Object.DestroyImmediate(obj); }
                catch { }
            };
        }
#endif
    }
}