using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(Modular3DText))]
    public class Modular3DTextEditor : Editor
    {
        Modular3DText myTarget;
        SerializedObject soTarget;

        SerializedProperty text;

        //main settings
        SerializedProperty font;
        SerializedProperty material;
        SerializedProperty fontSize;

        SerializedProperty autoLetterSize;
        SerializedProperty _wordSpacing;


        //effects
        SerializedProperty useModules;
        SerializedProperty startAddingModuleFromChar;
        SerializedProperty addingModules;
        SerializedProperty startDeletingModuleFromChar;
        SerializedProperty deletingModules;
        SerializedProperty customDeleteAfterDuration;
        SerializedProperty deleteAfter;
        SerializedProperty runModulesOnInstantiate;
        SerializedProperty runModulesOnEnable;

        //advanced settings
        SerializedProperty destroyChildObjectsWithGameObject;
        SerializedProperty repositionOldCharacters;
        SerializedProperty reApplyModulesToOldCharacters;
        //SerializedProperty activateChildObjects;

        SerializedProperty hideOverwrittenVariablesFromInspector;
        SerializedProperty combineMeshInEditor;
        SerializedProperty singleInPrefab;
        SerializedProperty combineMeshDuringRuntime;
        SerializedProperty hideLettersInHierarchyInPlayMode;
        SerializedProperty updateTextOncePerFrame;
        SerializedProperty autoSaveMesh;
        //SerializedProperty saveObjectInScene;
        
        //Debug -- starts
        SerializedProperty wordArray;


        //Debug --- ends

        SerializedProperty canBreakOutermostPrefab;

        SerializedProperty debugLogs;

        AnimBool showAdvancedSettingsInEditor;
        AnimBool showDebugSettingsInEditor;
        AnimBool showMainSettingsInEditor;
        AnimBool showLayoutSettingsInEditor;
        AnimBool showModuleSettingsInEditor;

        //style
        private static GUIStyle toggleStyle = null;
        private static GUIStyle foldOutStyle = null;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);
        private static Color toggledOnButtonColor = Color.white;
        private static Color toggledOffButtonColor = Color.gray;
        //private static Color toggledOffButtonColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1);

        readonly string addingtoolTip = "During runtime, these modules are called when new characters are added to the text.";
        readonly string deleteingtoolTip = "During runtime, these modules are called when characters are removed from the text.";


        void OnEnable()
        {
            myTarget = (Modular3DText)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();

            EditorGUI.BeginChangeCheck();

            WarningCheck();

            EditorGUILayout.PropertyField(text, GUIContent.none, GUILayout.Height(100));

            GUILayout.Space(5);

            MainSettings();

            GUILayout.Space(5);

            LayoutSettings(out Alignment anchor, out Vector3 spacing, out float width, out float height);

            GUILayout.Space(5);

            ModuleSettings();

            GUILayout.Space(5);

            AdvancedSettings();

            GUILayout.Space(5);

            DebugView();

            if (EditorGUI.EndChangeCheck())
            {
                MText_Font font = myTarget.Font;
                string text = myTarget.Text;

                ApplyGridLayoutSettings(anchor, spacing, width, height);

                //In prefab mode font change wasn't updating for some reasons
                if (font != myTarget.Font || soTarget.ApplyModifiedProperties())
                {
                    if (text == myTarget.Text)
                        myTarget.oldText = "";

                    myTarget.updatedAfterStyleUpdateOnPrefabInstances = false;
                    myTarget.UpdateText();
                }

                EditorUtility.SetDirty(myTarget);
            }
        }

        private void ApplyGridLayoutSettings(Alignment anchor, Vector2 spacing, float width, float height)
        {
            GridLayoutGroup gridLayoutGroup = myTarget.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
                return;

            if (anchor != gridLayoutGroup.Anchor)
                gridLayoutGroup.Anchor = anchor;

            if (spacing != gridLayoutGroup.Spacing)
                gridLayoutGroup.Spacing = spacing;

            if (width != gridLayoutGroup.Width)
                gridLayoutGroup.Width = width;
            if (height != gridLayoutGroup.Height)
                gridLayoutGroup.Height = height;
        }

        private void WarningCheck()
        {
            EditorGUI.indentLevel = 0;
            if (!myTarget.Font) EditorGUILayout.HelpBox("No font selected", MessageType.Error);
            if (!myTarget.Material) EditorGUILayout.HelpBox("No material selected", MessageType.Error);
            if (myTarget.DoesStyleInheritFromAParent()) EditorGUILayout.HelpBox("Some values are overwritten by parent button/list.", MessageType.Info);
        }



        #region Main Sections
        private void MainSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showMainSettingsInEditor.target = EditorGUILayout.Foldout(showMainSettingsInEditor.target, "Main Settings", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showMainSettingsInEditor.faded))
            {
                //DrawUILine(blueFaded);
                EditorGUI.indentLevel = 1;

                GUILayout.Space(5);
                MText_Editor_Methods.HorizontalField(font, "Font", "", FieldSize.small);
                if (!myTarget.DoesStyleInheritFromAParent() || !myTarget.hideOverwrittenVariablesFromInspector)
                {
                    MText_Editor_Methods.HorizontalField(material, "Material", "", FieldSize.small);
                    MText_Editor_Methods.HorizontalField(fontSize, "Size", "", FieldSize.small);
                }

                CombineMesh();

                EditorGUI.indentLevel = 3;
                DontCombineInEditorEither();
                EditorGUI.indentLevel = 0;

                GUILayout.Space(10);

                TextStyles();

            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void LayoutSettings(out Alignment anchor, out Vector3 spacing, out float width, out float height)
        {
            GridLayoutGroup gridLayout = myTarget.gameObject.GetComponent<GridLayoutGroup>();
            if (!gridLayout)
            {
                anchor = Alignment.MiddleCenter;
                spacing = Vector3.zero;
                width = 0;
                height = 0;
            }
            else
            {
                anchor = gridLayout.Anchor;
                spacing = gridLayout.Spacing;
                width = gridLayout.Width;
                height = gridLayout.Height;
            }

            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showLayoutSettingsInEditor.target = EditorGUILayout.Foldout(showLayoutSettingsInEditor.target, new GUIContent("Layout Settings", "Layouts are driven by Layout Groups. Although grid layout groups is the default one, it can work with any layout group. Experiment with different ones."), true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showLayoutSettingsInEditor.faded))
            {
                GUILayout.Space(5);
                MText_Editor_Methods.HorizontalField(autoLetterSize, "Auto Letter Size", "If turned on, instead of using predetermined size of each letter, their max X size is taken from the size they take in render view", FieldSize.extraLarge);
                EditorGUILayout.PropertyField(_wordSpacing);
                GUILayout.Space(5);

                ChooseLayoutGroups();

                if (myTarget.gameObject.GetComponent<GridLayoutGroup>())
                {
                    //GUILayout.Space(5);
                    //anchor = (Alignment)EditorGUILayout.EnumPopup(myTarget.GetComponent<GridLayoutGroup>().Anchor);

                    //GUILayout.BeginHorizontal();
                    //GUILayout.Label("", GUILayout.MaxWidth(9));
                    //GUILayout.Label("Spacing", GUILayout.MaxWidth(55));
                    //spacing = EditorGUILayout.Vector3Field(GUIContent.none, myTarget.GetComponent<GridLayoutGroup>().Spacing, GUILayout.MinWidth(80));
                    //GUILayout.EndHorizontal();



                    /////Height width
                    //GUILayout.BeginHorizontal();
                    //GUILayout.Label("", GUILayout.MaxWidth(9));
                    //GUILayout.Label("Width", GUILayout.MaxWidth(36));
                    //width = EditorGUILayout.FloatField(GUIContent.none, myTarget.GetComponent<GridLayoutGroup>().Width, GUILayout.MinWidth(60));
                    //GUILayout.Label("Height", GUILayout.MaxWidth(42));
                    //height = EditorGUILayout.FloatField(GUIContent.none, myTarget.GetComponent<GridLayoutGroup>().Height, GUILayout.MinWidth(60));
                    //GUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("Modify the attached layout group to modify the Layout of the text.", MessageType.Info);
                }
                else if (myTarget.gameObject.GetComponent<LayoutGroup>())
                {
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.HelpBox("Modify the attached layout group to modify the Layout of the text.", MessageType.Info);
                }
                else
                {
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.HelpBox("No layout group seems to be attached to the text. If this is intentional, ignore this message. Otherwise, please add any layout group to this object. Grid Layout group is the default one.", MessageType.Warning);
                    if (GUILayout.Button("Add grid layout Group"))
                    {
                        myTarget.gameObject.AddComponent<GridLayoutGroup>();
                        //TODO: Bug: the width and height doesn't change
                        //myTarget.gameObject.GetComponent<GridLayoutGroup>().Width = 5;
                        //myTarget.gameObject.GetComponent<GridLayoutGroup>().Height = 3;
                    }
                }
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void ModuleSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useModules, GUIContent.none, GUILayout.MaxWidth(25));
            showModuleSettingsInEditor.target = EditorGUILayout.Foldout(showModuleSettingsInEditor.target, new GUIContent("Modules", "Modules provide an easy way animate characters. \n They are called in two events. \n1. When new characters are added to text. \n2. When a character is removed from the text. \nThis change can be done by code or ui or anything. The text string is a property."), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            if (EditorGUILayout.BeginFadeGroup(showModuleSettingsInEditor.faded))
            {

                if (myTarget.combineMeshDuringRuntime)
                    EditorGUILayout.HelpBox("Text combine in playmode/build is turned on.\nThis means modules won't work.", MessageType.Warning);
              
                float defaultWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 170;
                EditorGUILayout.PropertyField(runModulesOnInstantiate);
                EditorGUILayout.PropertyField(runModulesOnEnable);
                EditorGUIUtility.labelWidth = defaultWidth;

                if (myTarget.runModulesOnInstantiate && myTarget.runModulesOnEnable)
                    EditorGUILayout.HelpBox("Both run module on instantiate and enabled is turned on. Only turn on Run Modules on Enable to avoid issues.", MessageType.Warning);

                EditorGUI.indentLevel = 2;
                ModuleContainerList("Adding", addingtoolTip, myTarget.addingModules, addingModules);
                GUILayout.Space(10);
                ModuleContainerList("Deleting", deleteingtoolTip, myTarget.deletingModules, deletingModules);
                GUILayout.Space(5);
                DeleteAfterDuration();

              
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void AdvancedSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showAdvancedSettingsInEditor.target = EditorGUILayout.Foldout(showAdvancedSettingsInEditor.target, "Advanced Settings", true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showAdvancedSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 1;

                MText_Editor_Methods.HorizontalField(hideOverwrittenVariablesFromInspector, "Hide overwritten values", "Texts under button/list sometimes have styles overwritten. This hides these variables", FieldSize.extraLarge);

                CombineMeshSettings();

                EditorGUI.indentLevel = 1;
                //EditorGUILayout.PropertyField(saveObjectInScene);

                EditorGUILayout.Space(5);
                //EditorGUILayout.PropertyField(repositionOldCharacters, new GUIContent("Reposition old Chars", "If old text = '123' and updated new text = '1234',\nthe '123' will be moved to their correct position when entering the '4'"));
                MText_Editor_Methods.HorizontalField(repositionOldCharacters, "Reposition old Chars", "If old text = '123' and updated new text = '1234',\nthe '123' will be moved to their correct position when entering the '4'", FieldSize.extraLarge);
                //EditorGUILayout.PropertyField(reApplyModulesToOldCharacters, new GUIContent("Re-apply modules", "If old text = old and updated new text = oldy,\ninstead of applying module to only 'y', it will apply to all chars"));
                MText_Editor_Methods.HorizontalField(reApplyModulesToOldCharacters, "Re-apply modules", "If old text = old and updated new text = oldy,\ninstead of applying module to only 'y', it will apply to all chars", FieldSize.extraLarge);
                //HorizontalFieldShortProperty(activateChildObjects, "Auto-activate ChildObjects", "", FieldSize.small);
                MText_Editor_Methods.HorizontalField(updateTextOncePerFrame, "Update once per frame", "If the gameobject is active in hierarchy, uses coroutine to make sure the text is only updated visually once per frame instead of wasting resources if updated multiple times by a script. This is only used if the game object is active in hierarchy and it updates at the end of frame.", FieldSize.extraLarge);
                EditorGUILayout.Space(5);
                MText_Editor_Methods.HorizontalField(startAddingModuleFromChar, "Start adding module from char", "If true, the adding module uses MonoBehavior attached to the char to run the coroutine. This way, if the text is deactivated, the module isn't interrupted.", FieldSize.gigantic);
                MText_Editor_Methods.HorizontalField(startDeletingModuleFromChar, "Start deleting module from char", "If true, the deleting module uses MonoBehavior attached to the char to run the coroutine. This way, if the text is deactivated, the module isn't interrupted.", FieldSize.gigantic);

                GUIContent hideLetters = new GUIContent("Hide letters in Hierarchy", "Hides the game object of letters in the hierarchy. They are still there just not visible. No impact except for cleaner hierarchy.");
                EditorGUILayout.LabelField(hideLetters);
                EditorGUI.indentLevel = 3;
                MText_Editor_Methods.HorizontalField(hideLettersInHierarchyInPlayMode, "In play mode", "When turned on, the letters created during playmode won't show-up in hierarchy. Has no impact in usage other than clean hierarchy.", FieldSize.large);
                EditorGUI.indentLevel = 1;

                EditorGUILayout.Space(10);

                PrefabAdvancedSettings();


                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(debugLogs);
                MText_Editor_Methods.HorizontalField(destroyChildObjectsWithGameObject, "Destroy Letter With this", "If you delete the gameobject, the letters are auto deleted also even if they aren't child object.", FieldSize.extraLarge);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void DebugView()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDebugSettingsInEditor.target = EditorGUILayout.Foldout(showDebugSettingsInEditor.target, "Debug", true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showDebugSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.PropertyField(wordArray);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            GUILayout.Space(15);
        }
        #endregion Main Sections


        private void ChooseLayoutGroups()
        {
            Color defaultColor = GUI.color;

            EditorGUILayout.BeginHorizontal();
            var groups = myTarget.GetListOfAllLayoutGroups();
            for (int i = 0; i < groups.Count; i++)
            {
                if (i == 0)
                {
                    if (myTarget.gameObject.GetComponent(groups[i]))
                        GUI.color = toggledOnButtonColor;
                    else
                        GUI.color = toggledOffButtonColor;

                    if (LeftButton(new GUIContent(FormatClassName(groups[i].Name))))
                    {
                        AddLayoutComponent(groups, i);
                    }
                }
                else if (i + 1 == groups.Count)
                {
                    if (myTarget.gameObject.GetComponent(groups[i]))
                        GUI.color = toggledOnButtonColor;
                    else
                        GUI.color = toggledOffButtonColor;

                    if (RightButton(new GUIContent(FormatClassName(groups[i].Name))))
                    {
                        AddLayoutComponent(groups, i);
                    }
                }
                else
                {
                    if (myTarget.gameObject.GetComponent(groups[i]))
                        GUI.color = toggledOnButtonColor;
                    else
                        GUI.color = toggledOffButtonColor;

                    if (MidButton(new GUIContent(FormatClassName(groups[i].Name))))
                    {
                        AddLayoutComponent(groups, i);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = defaultColor;
        }

        private void AddLayoutComponent(List<System.Type> groups, int i)
        {
            if (myTarget.GetComponent(groups[i]))
                return;

            string title = "Change Layout Group to " + FormatClassName(groups[i].Name);
            string mainDialogue = "This will overwrite all current layout settings";

            bool agreed = EditorUtility.DisplayDialog(title, mainDialogue, "Continue", "Cancel");
            if (agreed)
            {
                DestroyImmediate(myTarget.gameObject.GetComponent<LayoutGroup>());
                myTarget.gameObject.AddComponent(groups[i]);
                EditorApplication.delayCall += () => myTarget.gameObject.GetComponent<Modular3DText>().CleanUpdateText();
            }
        }

        string FormatClassName(string name)
        {
            if (name == "GridLayoutGroup")
                return "Grid";
            if (name == "CircularLayoutGroup")
                return "Circular";
            if (name == "LinearLayoutGroup")
                return "Linear";
            return name;
        }

        #region Functions for main settings
        /// <summary>
        /// Direction, capitalize etc.
        /// </summary>
        private void TextStyles()
        {
            EditorGUILayout.BeginHorizontal();

            Color original = GUI.backgroundColor;

            if (myTarget.LowerCase)
                GUI.backgroundColor = toggledOnButtonColor;
            else
                GUI.backgroundColor = toggledOffButtonColor;

            GUIContent smallCase = new GUIContent("ab", "lower case");
            if (LeftButton(smallCase))
            {
                myTarget.LowerCase = !myTarget.LowerCase;
                myTarget.Capitalize = false;
                myTarget.UpdateText();
                EditorUtility.SetDirty(myTarget);
            }


            if (myTarget.Capitalize)
                GUI.backgroundColor = toggledOnButtonColor;
            else
                GUI.backgroundColor = toggledOffButtonColor;

            GUIContent capitalize = new GUIContent("AB", "UPPER CASE");
            if (RightButton(capitalize))
            {
                myTarget.Capitalize = !myTarget.Capitalize;
                myTarget.LowerCase = false;
                myTarget.UpdateText();
                EditorUtility.SetDirty(myTarget);
            }

            GUI.backgroundColor = original;
            EditorGUILayout.EndHorizontal();
        }
        #endregion Functions for main settings


        #region Functions for module settings
        private void DeleteAfterDuration()
        {
            string toolTip = "Determines when a character is removed from text, how long it takes to be deleted. \nIf set to false, when a character is deleted, it is removed instantly or after the highest duration of modules, if there is any. \n Ignored if modules are disabled.";

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(customDeleteAfterDuration, GUIContent.none, GUILayout.MaxWidth(30));
            if (!myTarget.customDeleteAfterDuration)
            {
                float duration = myTarget.GetDeleteDurationFromEffects();
                GUIContent content = new GUIContent("Delete After : " + duration + " seconds", toolTip);
                EditorGUILayout.LabelField(content);
            }
            else
            {
                MText_Editor_Methods.HorizontalField(deleteAfter, "Delete After", toolTip, FieldSize.normal);
                GUIContent content = new GUIContent(" seconds", toolTip);
                EditorGUILayout.LabelField(content);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ModuleContainerList(string label, string tooltip, List<MText_ModuleContainer> moduleContainers, SerializedProperty serializedContainer)
        {
            Color original = GUI.backgroundColor;
            Color originalContent = GUI.contentColor;


            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            EditorGUILayout.LabelField(new GUIContent(label, tooltip));
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUI.backgroundColor = Color.white;
            GUI.contentColor = originalContent;
            GUIContent deleteIcon = EditorGUIUtility.IconContent("d_winbtn_win_close");

            for (int i = 0; i < moduleContainers.Count; i++)
            {
                if (i % 2 != 0)
                    GUI.backgroundColor = openedFoldoutTitleColor;
                else
                    GUI.backgroundColor = toggledOnButtonColor;


                GUILayout.BeginVertical("Box");


                //GUILayout.BeginVertical("CN EntryBackEven");
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedContainer.GetArrayElementAtIndex(i).FindPropertyRelative("module"), GUIContent.none, GUILayout.MinWidth(10));
                //EditorGUILayout.PropertyField(serializedContainer.GetArrayElementAtIndex(i).FindPropertyRelative("duration"), GUIContent.none, GUILayout.MaxWidth(65));

                if (GUILayout.Button(deleteIcon, GUILayout.MinHeight(20), GUILayout.MaxWidth(20)))
                {
                    moduleContainers.RemoveAt(i);
                }
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel = 1;
                if (i < moduleContainers.Count)
                {
                    if (moduleContainers[i].module != null)
                    {
                        if (moduleContainers[i].variableHolders != null)
                        {
                            if (moduleContainers[i].module.variableHolders != null)
                            {
                                if (moduleContainers[i].variableHolders.Length != moduleContainers[i].module.variableHolders.Length)
                                {
                                    VariableHolder[] newHolder = new VariableHolder[moduleContainers[i].module.variableHolders.Length];
                                    for (int k = 0; k < newHolder.Length; k++)
                                    {
                                        if (k < moduleContainers[i].variableHolders.Length)
                                        {
                                            newHolder[k] = moduleContainers[i].variableHolders[k];
                                        }
                                    }
                                    moduleContainers[i].variableHolders = newHolder;
                                }
                            }

                            for (int j = 0; j < moduleContainers[i].variableHolders.Length; j++)
                            {
                                DrawVariableHolder(moduleContainers, serializedContainer, i, j);
                            }

                            string warning = moduleContainers[i].module.VariableWarnings(moduleContainers[i].variableHolders);
                            if (warning != null)
                            {
                                if (warning.Length > 0)
                                {
                                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                                }
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
                if (i + 1 != moduleContainers.Count)
                {
                    EditorGUILayout.Space(5);
                }
            }
            GUI.contentColor = Color.black;
            GUI.backgroundColor = original;
            //GUIContent addIcon = EditorGUIUtility.IconContent("d_TreeEditor.Trash");
            if (GUILayout.Button("Add New Module", GUILayout.MinHeight(20)))
            {
                myTarget.EmptyEffect(moduleContainers);
            }

            GUI.contentColor = originalContent;

            GUILayout.EndVertical();
        }

        private void DrawVariableHolder(List<MText_ModuleContainer> moduleContainers, SerializedProperty serializedContainer, int i, int j)
        {
            if (moduleContainers[i].module.variableHolders != null)
            {
                if (!ShowProperty(moduleContainers[i].module.variableHolders, j, moduleContainers[i].variableHolders))
                    return;

                GUIContent variableLabel = null;
                if (moduleContainers[i].module.variableHolders[j].variableName != null)
                    if (moduleContainers[i].module.variableHolders[j].variableName != string.Empty)
                        variableLabel = new GUIContent(moduleContainers[i].module.variableHolders[j].variableName);

                if (variableLabel == null)
                    variableLabel = new GUIContent("Unlabeled variable");

                ModuleVariableType type = moduleContainers[i].module.variableHolders[j].type;

                SerializedProperty property;
                string propertyName = MText_Editor_Methods.GetPropertyName(type);

                if (moduleContainers != null)
                {
                    if (serializedContainer.arraySize > i)
                    {
                        if (serializedContainer.GetArrayElementAtIndex(i).FindPropertyRelative("variableHolders").arraySize > j)
                        {
                            property = serializedContainer.GetArrayElementAtIndex(i).FindPropertyRelative("variableHolders").GetArrayElementAtIndex(j).FindPropertyRelative(propertyName);
                            EditorGUILayout.PropertyField(property, variableLabel);
                        }
                    }
                }
            }
        }

        //should check from the module in module container list
        private bool ShowProperty(VariableHolder[] moduleVariables, int i, VariableHolder[] textVariables)
        {
            if (moduleVariables[i].hideIf == null)
                return true;

            if (!string.IsNullOrEmpty(moduleVariables[i].hideIf))
            {
                for (int j = 0; j < moduleVariables.Length; j++)
                {
                    if (j == i)
                        continue;

                    if (moduleVariables[j].type == ModuleVariableType.@bool)
                    {
                        if (moduleVariables[j].variableName == moduleVariables[i].hideIf)
                        {
                            if (textVariables[j] == null)
                                return true;

                            if (textVariables[j].boolValue == true)
                                return false;
                            else
                                return true;
                        }
                    }
                }
            }
            return true;
        }

    
        #endregion Functions for module settings


        #region Functions for advanced settings        
        private void PrefabAdvancedSettings()
        {
            if (PrefabUtility.IsPartOfPrefabInstance(myTarget.gameObject))
            {
                if (PrefabUtility.IsOutermostPrefabInstanceRoot(myTarget.gameObject))
                {
                    MText_Editor_Methods.HorizontalField(canBreakOutermostPrefab, "Break outermost Prefab", "If the text isn't a child object of the prefab, it can break prefab and save the reference.", FieldSize.extraLarge);
                }
            }
            else MeshSaveSettings();
            PrefabMeshSaveSettings();
        }
        private void PrefabMeshSaveSettings()
        {
            if (myTarget.assetPath != "" && myTarget.assetPath != null && !EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField(myTarget.assetPath, EditorStyles.boldLabel);
                if (GUILayout.Button("Apply to prefab"))
                {
                    myTarget.ReconnectPrefabs();
                }
            }

            if ((myTarget.assetPath != "" && myTarget.assetPath != null && !EditorApplication.isPlaying))
            {
                if (GUILayout.Button("Remove prefab connection"))
                {
                    myTarget.assetPath = "";
                }
            }
            if (PrefabUtility.IsPartOfPrefabInstance(myTarget.gameObject))
            {
                MeshSaveSettings();
            }
        }
        #endregion Functions for advanced settings

        private void CombineMesh()
        {
            EditorGUILayout.LabelField(new GUIContent("Single mesh", "Note that there is a maximum amount of verticies each mesh can handle in unity.If the limit is exceeded, rest of the text will be moved to a child. The limit is insanely high and shouldn't be an issue."), GUILayout.MaxWidth(92));
            EditorGUI.indentLevel = 3;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(combineMeshInEditor, new GUIContent("Editor", "Combine into a single mesh in Editor."));
            EditorGUILayout.PropertyField(combineMeshDuringRuntime, new GUIContent("Play mode/Build", "There is no reason to turn this on unless you really need this for something. \nOtherwise, wasted resource on combining"));
            EditorGUILayout.EndHorizontal();

        }
        private void CombineMeshSettings()
        {
            //EditorGUILayout.BeginHorizontal();
            //{
            //    string singleMeshEditorTooltip = "The same option as in the Main Setting\n" +
            //        "Combine into a single mesh in Editor, edit mode\n" +
            //        "Note that there is a maximum amount of verticies each mesh can handle. If the limit is exceeded, rest of the text will be moved to a child.";
            //    EditorGUILayout.LabelField(new GUIContent("Single mesh in Editor", singleMeshEditorTooltip), GUILayout.Width(139));
            //    EditorGUILayout.PropertyField(combineMeshInEditor, GUIContent.none, GUILayout.Width(20));

            //    EditorGUILayout.PropertyField(combineMeshDuringRuntime, new GUIContent("in Play mode", "There is no reason to turn this on unless you really need this for something. \nOtherwise, wasted resource on combining"));
            //}
            //EditorGUILayout.EndHorizontal();
            //EditorGUI.indentLevel = 3;

            DontCombineInEditorEither();

            if (myTarget.gameObject.GetComponent<MeshFilter>())
            {
                if (GUILayout.Button(new GUIContent("Optimize mesh", "This causes the geometry and vertices of the combined mesh to be reordered internally in an attempt to improve vertex cache utilisation on the graphics hardware and thus rendering performance. This operation can take a few seconds or more for complex meshes.")))
                {
                    MText_Utilities.OptimizeMesh(myTarget.gameObject.GetComponent<MeshFilter>().sharedMesh);
                }
            }
        }
        private void MeshSaveSettings()
        {
            if (myTarget.gameObject.GetComponent<MeshFilter>())
            {
                GUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(autoSaveMesh);

                GUILayout.BeginHorizontal();
                if (!myTarget.autoSaveMesh)
                {
                    if (GUILayout.Button(new GUIContent("Save mesh")))
                    {
                        myTarget.SaveMeshAsAsset(false);
                    }
                }
                if (GUILayout.Button(new GUIContent("Save mesh as", "Save a new copy of the mesh in project")))
                {
                    myTarget.SaveMeshAsAsset(true);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
        private void DontCombineInEditorEither()
        {
            if (!myTarget.combineMeshInEditor && PrefabUtility.IsPartOfPrefabInstance(myTarget.gameObject))
            {
                GUILayout.BeginHorizontal();

                string tooltip = "Prefabs don't allow child objects that are part of the prefab to be deleted in Editor.\n" +
                    "If you add child objects, then apply, which adds these child objects to the prefab,\n" +
                    "When changing text again, this script can't delete the old gameobjects. Just disables them. Remember to clean them up manually if you enable this.";
                EditorGUILayout.LabelField(new GUIContent("Single in Prefab", tooltip), GUILayout.Width(140));
                EditorGUILayout.PropertyField(singleInPrefab, GUIContent.none);

                //                EditorGUILayout.PropertyField(dontCombineInEditorAnyway, new GUIContent("Single in Prefab too", tooltip), GUILayout.Width(400));
                GUILayout.EndHorizontal();
            }
        }






        #region Style
        void GenerateStyle()
        {
            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(GUI.skin.button);
                toggleStyle.margin = new RectOffset(0, 0, toggleStyle.margin.top, toggleStyle.margin.bottom);
            }

            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }


            EditorStyles.popup.fontSize = 11;
            EditorStyles.popup.fixedHeight = 18;
        }
        bool LeftButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(0, 0, rect.width + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;

            GUI.EndGroup();
            return clicked;
        }
        bool MidButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);


            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
                //if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }
        bool RightButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Called on Enable
        /// </summary>
        private void FindProperties()
        {
            showMainSettingsInEditor = new AnimBool(true);
            showMainSettingsInEditor.valueChanged.AddListener(Repaint);
            showLayoutSettingsInEditor = new AnimBool(false);
            showLayoutSettingsInEditor.valueChanged.AddListener(Repaint);
            showModuleSettingsInEditor = new AnimBool(false);
            showModuleSettingsInEditor.valueChanged.AddListener(Repaint);            
            showAdvancedSettingsInEditor = new AnimBool(false);
            showAdvancedSettingsInEditor.valueChanged.AddListener(Repaint);
            showDebugSettingsInEditor = new AnimBool(false);
            showDebugSettingsInEditor.valueChanged.AddListener(Repaint);

            text = soTarget.FindProperty("_text");

            autoSaveMesh = soTarget.FindProperty("autoSaveMesh");

            //main settings
            font = soTarget.FindProperty("_font");
            material = soTarget.FindProperty("_material");
            fontSize = soTarget.FindProperty("_fontSize");

            autoLetterSize = soTarget.FindProperty("_autoLetterSize");
            _wordSpacing = soTarget.FindProperty("_wordSpacing");

            //effects
            useModules = soTarget.FindProperty("useModules");
            startAddingModuleFromChar = soTarget.FindProperty("startAddingModuleFromChar");
            addingModules = soTarget.FindProperty("addingModules");
            startDeletingModuleFromChar = soTarget.FindProperty("startDeletingModuleFromChar");
            deletingModules = soTarget.FindProperty("deletingModules");
            customDeleteAfterDuration = soTarget.FindProperty("customDeleteAfterDuration");
            deleteAfter = soTarget.FindProperty("deleteAfter");
            runModulesOnInstantiate = soTarget.FindProperty("runModulesOnInstantiate");
            runModulesOnEnable = soTarget.FindProperty("runModulesOnEnable");

            //advanced
            destroyChildObjectsWithGameObject = soTarget.FindProperty("destroyChildObjectsWithGameObject");
            repositionOldCharacters = soTarget.FindProperty("repositionOldCharacters");
            reApplyModulesToOldCharacters = soTarget.FindProperty("reApplyModulesToOldCharacters");
            //activateChildObjects = soTarget.FindProperty("activateChildObjects");

            hideOverwrittenVariablesFromInspector = soTarget.FindProperty("hideOverwrittenVariablesFromInspector");
            combineMeshInEditor = soTarget.FindProperty("combineMeshInEditor");
            singleInPrefab = soTarget.FindProperty("singleInPrefab");
            combineMeshDuringRuntime = soTarget.FindProperty("combineMeshDuringRuntime");
            hideLettersInHierarchyInPlayMode = soTarget.FindProperty("hideLettersInHierarchyInPlayMode");
            //hideLettersInHierarchyInEditMode = soTarget.FindProperty("hideLettersInHierarchyInEditMode");
            updateTextOncePerFrame = soTarget.FindProperty("updateTextOncePerFrame");


            canBreakOutermostPrefab = soTarget.FindProperty("canBreakOutermostPrefab");
            //saveObjectInScene = soTarget.FindProperty("saveObjectInScene");
            debugLogs = soTarget.FindProperty("debugLogs");


            wordArray = soTarget.FindProperty("wordArray");
        }
        #endregion Functions
    }
}