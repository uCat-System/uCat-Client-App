using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
/// Created by Ferdowsur Asif @ Tiny Giant Studio
using UnityEngine;

namespace MText
{
    /// <summary>
    /// TODO: Loop through all characters and check if any is empty
    /// </summary>

    [CustomEditor(typeof(MText_Font))]
    public class MText_FontEditor : Editor
    {
        MText_Font myTarget;
        SerializedObject soTarget;


        SerializedProperty modelSource;
        SerializedProperty characters;
        SerializedProperty showCharacterDetailsEditor;

        SerializedProperty monoSpaceFont;
        SerializedProperty monoSpaceSpacing;
        SerializedProperty unitPerEM;
        SerializedProperty useUpperCaseLettersIfLowerCaseIsMissing;
        SerializedProperty spaceSpacing; //word spacing?
        SerializedProperty characterSpacing;
        SerializedProperty lineHeight;


        SerializedProperty fallbackFonts;

        SerializedProperty enableKerning;
        SerializedProperty kerningMultiplier;
        SerializedProperty kernTable;


        private static GUIStyle foldOutStyle = null;
        private static GUIStyle strongLabel = null;
        private static GUIStyle miniLabel = null;
        private static GUIStyle searchField = null;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);


        private AnimBool showFallBackSettingsInEditor;
        private AnimBool showSpacingSettingsInEditor;
        private AnimBool showBaseSettingsInEditor;
        private AnimBool showKerningTableInEditor;
        private AnimBool showCharacterListInEditor;


        void OnEnable()
        {
            myTarget = (MText_Font)target;
            soTarget = new SerializedObject(target);

            showFallBackSettingsInEditor = new AnimBool(false);
            showFallBackSettingsInEditor.valueChanged.AddListener(Repaint);
            showSpacingSettingsInEditor = new AnimBool(false);
            showSpacingSettingsInEditor.valueChanged.AddListener(Repaint);
            showBaseSettingsInEditor = new AnimBool(false);
            showBaseSettingsInEditor.valueChanged.AddListener(Repaint);
            showKerningTableInEditor = new AnimBool(false);
            showKerningTableInEditor.valueChanged.AddListener(Repaint);
            showCharacterListInEditor = new AnimBool(false);
            showCharacterListInEditor.valueChanged.AddListener(Repaint);

            modelSource = soTarget.FindProperty("modelSource");
            useUpperCaseLettersIfLowerCaseIsMissing = soTarget.FindProperty("useUpperCaseLettersIfLowerCaseIsMissing");

            unitPerEM = soTarget.FindProperty("unitPerEM");
            monoSpaceFont = soTarget.FindProperty("monoSpaceFont");
            monoSpaceSpacing = soTarget.FindProperty("monoSpaceSpacing");
            spaceSpacing = soTarget.FindProperty("emptySpaceSpacing");
            characterSpacing = soTarget.FindProperty("characterSpacing");
            lineHeight = soTarget.FindProperty("lineHeight");


            characters = soTarget.FindProperty("characters");
            showCharacterDetailsEditor = soTarget.FindProperty("showCharacterDetailsEditor");
            fallbackFonts = soTarget.FindProperty("fallbackFonts");

            enableKerning = soTarget.FindProperty("enableKerning");
            kerningMultiplier = soTarget.FindProperty("kerningMultiplier");
            kernTable = soTarget.FindProperty("kernTable");
        }



        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();
            EditorGUI.BeginChangeCheck();


            WarningsCheck();


            GUILayout.Space(5);
            FallBackFont();
            GUILayout.Space(5);
            SpacingSettings();
            GUILayout.Space(5);
            CharacterSettings();
            GUILayout.Space(5);
            CharacterList();
            GUILayout.Space(5);
            KerningSettings();


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                ApplyFontChanges();
            }
        }





        private void WarningsCheck()
        {
            //if (myTarget.emptySpaceSpacing > 10) EditorGUILayout.HelpBox("Base spacing seems to be set too high. If it's intended, please ignore this message.", MessageType.Warning);
            //if (myTarget.characterSpacing > 10) EditorGUILayout.HelpBox("Character spacing seems to be set too high. If it's intended, please ignore this message.", MessageType.Warning);
        }


        private void FallBackFont()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);

            GUIContent fallBackFoldoutTitle = new GUIContent("Fallback Fonts", "If this font has missing characters, it will try to get the character from fallback font");
            showFallBackSettingsInEditor.target = EditorGUILayout.Foldout(showFallBackSettingsInEditor.target, fallBackFoldoutTitle, true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showFallBackSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 0;
                for (int i = 0; i < myTarget.fallbackFonts.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    if (fallbackFonts.arraySize > i)
                    {
                        if (myTarget.fallbackFonts[i] == myTarget)
                        {
                            Debug.LogError("Unnecessary self reference found on fallback font :" + i, myTarget);
                            myTarget.fallbackFonts.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(fallbackFonts.GetArrayElementAtIndex(i), GUIContent.none);

                            if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
                            {
                                Repaint();
                                myTarget.fallbackFonts.RemoveAt(i);
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("+"))
                {
                    myTarget.fallbackFonts.Add(null);
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void SpacingSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showSpacingSettingsInEditor.target = EditorGUILayout.Foldout(showSpacingSettingsInEditor.target, "Spacing", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSpacingSettingsInEditor.faded))
            {
                EditorGUILayout.PropertyField(unitPerEM);
                EditorGUILayout.PropertyField(characterSpacing, new GUIContent("Character", "Text's character spacing = font's character spacing * text's character spacing"));
                EditorGUILayout.PropertyField(spaceSpacing, new GUIContent("Word", "Word spacing and spacing for unavailable characters"));
                EditorGUILayout.PropertyField(lineHeight);
                EditorGUILayout.PropertyField(useUpperCaseLettersIfLowerCaseIsMissing, new GUIContent("Uppercase if missing lower"));
                EditorGUILayout.PropertyField(monoSpaceFont, new GUIContent("Mono Space", "Monospace means all characters are spaced equally.\nIf turned on, individual spacing value from list below is ignored. The information is not removed to avoid accidentally turning it on ruin the font. \nCharacter spacing is used for everything.\nThis is not used if auto letter size is turned on."));
                EditorGUILayout.PropertyField(monoSpaceSpacing);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void CharacterSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showBaseSettingsInEditor.target = EditorGUILayout.Foldout(showBaseSettingsInEditor.target, "Data", true, foldOutStyle);

            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showBaseSettingsInEditor.faded))
            {
                GetFontSet();
                GetFontBytes();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void GetFontBytes()
        {
            if (myTarget.fontBytes != null)
            {
                if (myTarget.fontBytes.Length > 0)
                    EditorGUILayout.LabelField("Font bytes: " + myTarget.fontBytes.Length);
                else
                    EditorGUILayout.LabelField("No TTF font bytes found.");
            }
            else
                EditorGUILayout.LabelField("No TTF font bytes found. It's completely fine. No missing character will be generated");


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select TTF font"))
            {
                string path = EditorUtility.OpenFilePanel("Select ttf font", "", "ttf");

                if (!string.IsNullOrEmpty(path))
                {
                    myTarget.SetFontBytes((File.ReadAllBytes(path)));
                    EditorUtility.SetDirty(myTarget);
                }
            }
            if (GUILayout.Button("Clear TTF data"))
            {
                myTarget.fontBytes = null;
            }
            GUILayout.EndHorizontal();
        }

        private void GetFontSet()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(modelSource);
            if (GUILayout.Button("Update characters"))
            {
                string mainDialogue = "How would you like to proceed";
                if (myTarget.modelSource == null)
                    mainDialogue = "Source model file is not found.\nOverwriting in this case will just clear the existing characters.\nHow would you like to proceed";

                int option = EditorUtility.DisplayDialogComplex("Update font character list from source model",
   mainDialogue,
   "Overwrite old character list",
   "Add to it",
   "Cancel");

                switch (option)
                {
                    // overwrite.
                    case 0:
                        myTarget.UpdateCharacterList(true);
                        EditorUtility.SetDirty(target);
                        break;

                    // Dont overwrite.
                    case 1:
                        myTarget.UpdateCharacterList(false);
                        EditorUtility.SetDirty(target);
                        break;
                    default:
                        break;
                }
            }
            GUILayout.EndHorizontal();
        }









        #region Character Set
        private void CharacterList()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showCharacterListInEditor.target = EditorGUILayout.Foldout(showCharacterListInEditor.target, "Character List", true, foldOutStyle);

            if (showCharacterListInEditor.value)
            {
                EditorGUILayout.PropertyField(showCharacterDetailsEditor, GUIContent.none, GUILayout.MaxWidth(30), GUILayout.MaxHeight(15));
                GUILayout.Label("Details", GUILayout.MaxWidth(43), GUILayout.MaxHeight(15));

                if (myTarget.beingSearched != null)
                {
                    if (myTarget.beingSearched.Length == 0)
                    {
                        int lastPage = (myTarget.characters.Count / characterCountInAPage) + 1;
                        PageNavigation(lastPage);
                    }
                }
            }
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

            //if (myTarget.showCharacterListInEditor)
            {
                if (EditorGUILayout.BeginFadeGroup(showCharacterListInEditor.faded))
                {
                    EditorGUI.indentLevel = 0;

                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(2), GUILayout.MaxHeight(15));
                    myTarget.beingSearched = GUILayout.TextField(myTarget.beingSearched, searchField);
                    GUILayout.EndHorizontal();


                    if (myTarget.beingSearched != null)
                    {
                        if (myTarget.beingSearched.Length == 0)
                            AllCreateCharacterList();
                        else
                            SearchedCharacterList();
                    }
                    else
                    {
                        AllCreateCharacterList();
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            GUILayout.EndVertical();
        }



        readonly int characterCountInAPage = 25;
        int currentPage;
        private void AllCreateCharacterList()
        {
            int startingNumber = currentPage * characterCountInAPage;
            if (startingNumber >= myTarget.characters.Count - 1)
                startingNumber = 0;
            int endingNumber = startingNumber + characterCountInAPage;
            if (endingNumber >= myTarget.characters.Count)
                endingNumber = myTarget.characters.Count;

            int lastPage = (myTarget.characters.Count / characterCountInAPage) + 1;

            //PageNavigation(lastPage);
            GUILayout.Space(5);

            EditorGUI.indentLevel = 0;
            for (int i = startingNumber; i < endingNumber; i++)
            {

                if (characters.arraySize > i)
                {
                    if (i != startingNumber)
                        GUILayout.Space(10);

                    CharacterItem(i);
                }

            }

            if (currentPage == Mathf.FloorToInt(myTarget.characters.Count / characterCountInAPage))
            {
                if (GUILayout.Button("+"))
                {
                    MText_Character character = new MText_Character();
                    myTarget.characters.Add(character);
                    EditorUtility.SetDirty(target);
                }
            }
            GUILayout.Space(5);

            PageNavigation(lastPage);
        }

        private void SearchedCharacterList()
        {
            for (int i = 0; i < myTarget.characters.Count; i++)
            {
                if (characters.arraySize > i)
                {
                    if (myTarget.beingSearched.Contains(myTarget.characters[i].character.ToString()))
                        CharacterItem(i);
                }

            }
        }

        private void CharacterItem(int i)
        {
            int size = 30;
            int spacing = 2;

            //Line 1
            GUILayout.BeginHorizontal();
            {
                Color contentColor = GUI.contentColor;
                GUI.contentColor = new Color(255 / 255f, 242 / 255f, 0 / 255f, 0.8f);

                GUILayout.BeginVertical(GUILayout.MaxWidth(size));
                {
                    GUILayout.Label(new GUIContent("Character", "The character this item will use."), strongLabel, GUILayout.MaxHeight(12), GUILayout.MaxWidth(65));
                    GUI.contentColor = new Color(255 / 255f, 242 / 255f, 0 / 255f, 1f);
                    EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("character"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(65));
                }
                GUILayout.EndVertical();
                GUI.contentColor = contentColor;


                RenderReferences(i, size);

                GUILayout.Label("", GUILayout.MaxWidth(spacing));


                GUIContent deleteIcon = EditorGUIUtility.IconContent("d_winbtn_win_close");
                try
                {
                    if (GUILayout.Button(deleteIcon, GUILayout.MaxWidth(size), GUILayout.MaxHeight(size)))
                    {
                        myTarget.characters.RemoveAt(i);
                    }
                }
                catch { }
            }
            GUILayout.EndHorizontal();

            if (myTarget.showCharacterDetailsEditor)
            {
                //Line 1
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.MaxWidth(size));

                CharacterWidth(i, size);
                //Offset(i, size);
                Offset(i);
                CharacterIndex(i, size);

                GUILayout.EndHorizontal();
            }

        }

        private void CharacterWidth(int i, int size)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(size * 2));
            GUILayout.Label(new GUIContent("Width", "Size of the character."), GUILayout.MaxHeight(15), GUILayout.MaxWidth(size * 2));
            EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("spacing"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(size * 2));
            GUILayout.EndVertical();
        }
        private void CharacterIndex(int i, int size)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(size * 2));
            GUILayout.Label(new GUIContent("Index", "Index of the character in typeface."), GUILayout.MaxHeight(15), GUILayout.MaxWidth(size * 2));
            EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("glyphIndex"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(size * 2));
            GUILayout.EndVertical();
        }
        private void RenderReferences(int i, int size)
        {
            if (i >= myTarget.characters.Count)
                return;

            if (myTarget.characters[i].prefab)
            {

                var texture = AssetPreview.GetAssetPreview(myTarget.characters[i].prefab);
                if (texture)
                    GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(size), GUILayout.MaxHeight(size));

                GUILayout.BeginVertical();
                GUILayout.Label("Prefab", GUILayout.MaxHeight(size / 2));
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("prefab"), GUIContent.none, GUILayout.MaxHeight(size / 2));
                GUILayout.EndVertical();
            }
            else if (myTarget.characters[i].meshPrefab)
            {

                var texture = AssetPreview.GetAssetPreview(myTarget.characters[i].meshPrefab);
                if (texture)
                    GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(size), GUILayout.MaxHeight(size));

                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("meshPrefab"), GUIContent.none, GUILayout.MaxHeight(size));
            }
            else
            {
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("prefab"), GUIContent.none, GUILayout.MaxHeight(size));
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("meshPrefab"), GUIContent.none, GUILayout.MaxHeight(size));
            }
        }

        //private void Offset(int i, int size)
        private void Offset(int i)
        {
            int labelWidth = 11;
            int propertyWidth = 35;

            GUILayout.BeginVertical();
            {
                GUILayout.Label("Offset", GUILayout.MaxHeight(12), GUILayout.MaxWidth(40));




                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent("X"), GUILayout.MaxHeight(15), GUILayout.MaxWidth(labelWidth));
                    EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("xOffset"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(propertyWidth));


                    GUILayout.Label(new GUIContent("Y"), GUILayout.MaxHeight(15), GUILayout.MaxWidth(labelWidth));
                    EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("yOffset"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(propertyWidth));


                    GUILayout.Label(new GUIContent("Z"), GUILayout.MaxHeight(15), GUILayout.MaxWidth(labelWidth));
                    EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("zOffset"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(propertyWidth));
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();
        }

        private void PageNavigation(int lastPage)
        {
            EditorGUI.indentLevel = 0;

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(75));
            GUIContent prev = EditorGUIUtility.IconContent("tab_prev");
            if (GUILayout.Button(prev, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                currentPage--;
                if (currentPage < 0)
                    currentPage = myTarget.characters.Count / characterCountInAPage;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.LabelField((currentPage + 1) + "/" + lastPage, GUILayout.MinWidth(40), GUILayout.MaxWidth(60));

            GUIContent next = EditorGUIUtility.IconContent("tab_next");
            if (GUILayout.Button(next, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                currentPage++;
                if (currentPage > myTarget.characters.Count / characterCountInAPage)
                    currentPage = 0;
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();
        }
        #endregion Character Set





        private void KerningSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUIContent kernTableFoldoutTitleContent = new GUIContent("Kerning Table", "Legacy 'kern' table");
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showKerningTableInEditor.target = EditorGUILayout.Foldout(showKerningTableInEditor.target, kernTableFoldoutTitleContent, true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showKerningTableInEditor.faded))
            {
                EditorGUILayout.PropertyField(enableKerning);
                EditorGUILayout.PropertyField(kerningMultiplier);
                DrawKernTable();
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void DrawKernTable()
        {
            EditorGUI.indentLevel = 1;
            if (myTarget.kernTable == null)
                return;
            if (myTarget.kernTable.Count == 0)
            {
                EditorGUILayout.LabelField(new GUIContent("No kern table found"));
                return;
            }

            int startingIndex = 0;
            int lastNumber = myTarget.kernTable.Count;
            //if (lastNumber > 10)
            //    lastNumber = 10;

            for (int i = startingIndex; i < lastNumber; i++)
            {
                DrawKerningElement(i);
            }
            //EditorGUILayout.PropertyField(kernTable);
        }

        private void DrawKerningElement(int i)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("Left: " + StartingChar(myTarget.kernTable[i].kernPair.left), ""), GUILayout.MaxHeight(15), GUILayout.MaxWidth(120));
            EditorGUILayout.LabelField(new GUIContent("Right: " + StartingChar(myTarget.kernTable[i].kernPair.right), ""), GUILayout.MaxHeight(15), GUILayout.MaxWidth(120));
            EditorGUILayout.LabelField(new GUIContent("Offset", ""), GUILayout.MaxHeight(15), GUILayout.MaxWidth(60));
            EditorGUILayout.PropertyField(kernTable.GetArrayElementAtIndex(i).FindPropertyRelative("offset"), GUIContent.none, GUILayout.MaxHeight(20), GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
        }
        string StartingChar(int index)
        {
            for (int j = 0; j < myTarget.characters.Count; j++)
            {
                if (myTarget.characters[j].glyphIndex == index)
                    return myTarget.characters[j].character.ToString();
            }
            return "Uknown";
        }

        //Called after fields are changed in inspector
        void ApplyFontChanges()
        {
            List<GameObject> allObjectInScene = GetAllObjectsOnlyInScene();
            List<Modular3DText> texts = new List<Modular3DText>();
            for (int i = 0; i < allObjectInScene.Count; i++)
            {
                if (allObjectInScene[i].GetComponent<Modular3DText>())
                    texts.Add(allObjectInScene[i].GetComponent<Modular3DText>());
            }

            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].Font == target)
                {
                    texts[i].CleanUpdateText();
                }
            }
        }

        List<GameObject> GetAllObjectsOnlyInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                    objectsInScene.Add(go);
            }

            return objectsInScene;
        }


        void GenerateStyle()
        {
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout);
                foldOutStyle.overflow = new RectOffset(-10, 0, 3, 0);
                foldOutStyle.padding = new RectOffset(15, 0, -3, 0);
                foldOutStyle.fontStyle = FontStyle.Bold;
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }

            if (strongLabel == null)
            {
                strongLabel = new GUIStyle(EditorStyles.boldLabel);
            }
            if (miniLabel == null)
            {
                miniLabel = new GUIStyle(EditorStyles.miniLabel);
            }
            if (searchField == null)
            {
                searchField = new GUIStyle(EditorStyles.toolbarSearchField);
            }
        }
    }
}
