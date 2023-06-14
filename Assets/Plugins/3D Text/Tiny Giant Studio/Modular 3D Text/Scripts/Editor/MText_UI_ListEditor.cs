using MText.EditorHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif



namespace MText
{
    [CustomEditor(typeof(MText_UI_List))]
    public class MText_UI_ListEditor : Editor
    {
        public MText_Settings settings;

        readonly float defaultSmallHorizontalFieldSize = 72.5f;
        readonly float defaultNormalltHorizontalFieldSize = 100;
        readonly float defaultLargeHorizontalFieldSize = 120f;
        readonly float defaultExtraLargeHorizontalFieldSize = 150f;

        //readonly string ignoreChildUnSelectModuleLabel = "Ignore Child Modules";
        //readonly string ignoreChildOnSelectModuleLabel = "Ignore Child Modules";
        //readonly string ignoreChildOnPressModuleLabel = "Ignore Child Modules";
        //readonly string ignoreChildOnClickModuleLabel = "Ignore Child Modules";


        MText_UI_List myTarget;
        SerializedObject soTarget;


        SerializedProperty autoFocusOnStart;
        SerializedProperty autoFocusFirstItem;



        SerializedProperty UseStyle;

        SerializedProperty useNormalItemVisual;
        SerializedProperty normalTextSize;
        SerializedProperty normalItemFontMaterial;
        SerializedProperty normalItemBackgroundMaterial;

        SerializedProperty useSelectedItemVisual;
        SerializedProperty selectedItemFontSize;
        SerializedProperty selectedItemFontMaterial;
        SerializedProperty selectedItemBackgroundMaterial;

        SerializedProperty usePressedItemVisual;
        SerializedProperty pressedItemFontSize;
        SerializedProperty pressedItemFontMaterial;
        SerializedProperty pressedItemBackgroundMaterial;
        SerializedProperty holdPressedVisualFor;

        SerializedProperty useDisabledItemVisual;
        SerializedProperty disabledItemFontSize;
        SerializedProperty disabledItemFontMaterial;
        SerializedProperty disabledItemBackgroundMaterial;


        SerializedProperty useModules;
        SerializedProperty ignoreChildModules;

        SerializedProperty ignoreChildUnSelectModuleContainers;
        SerializedProperty applyUnSelectModuleContainers;
        SerializedProperty unSelectModuleContainers;

        SerializedProperty ignoreChildOnSelectModuleContainers;
        SerializedProperty applyOnSelectModuleContainers;
        SerializedProperty onSelectModuleContainers;

        SerializedProperty ignoreChildOnPressModuleContainers;
        SerializedProperty applyOnPressModuleContainers;
        SerializedProperty onPressModuleContainers;

        SerializedProperty ignoreChildOnClickModuleContainers;
        SerializedProperty applyOnClickModuleContainers;
        SerializedProperty onClickModuleContainers;


        private static Color toggledOnButtonColor = Color.white;
        private static Color toggledOffButtonColor = Color.gray;
        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);


        //debug
        SerializedProperty selectedItem;


        AnimBool showLayoutSettingsInEditor;
        AnimBool showModuleSettingsInEditor;
        AnimBool showDebugSettingsInEditor;
        AnimBool showDisabledItemSettings;
        AnimBool showPressedItemSettings;
        AnimBool showSelectedItemSettings;
        AnimBool showNormalItemSettings;
        AnimBool showChildVisualSettings;






        enum FieldSize
        {
            small,
            normal,
            large,
            extraLarge
        }

        private static GUIStyle foldOutStyle = null;

        readonly private int spaceAtTheBottomOfABox = 6;

        private void Awake()
        {
            Undo.undoRedoPerformed += UndoRedo;
        }
        private void OnEnable()
        {
            myTarget = (MText_UI_List)target;
            soTarget = new SerializedObject(target);

            GetReferences();

            if (!settings)
                settings = MText_FindResource.VerifySettings(settings);
        }
        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            MainSettings();

            GUILayout.Space(10);
            VisualSettings();
            GUILayout.Space(5);
            ModuleSettings();
            GUILayout.Space(5);
            InputSettings();
            GUILayout.Space(5);
            DebugSettings();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();

                if (!EditorApplication.isPlaying)
                    myTarget.UnselectEverything();
                else
                    myTarget.UpdateStyle();

                EditorUtility.SetDirty(myTarget);
            }
        }

        void UndoRedo()
        {
            if (myTarget == null)
                myTarget = (MText_UI_List)target;

            if (myTarget)
                myTarget.UpdateStyle();
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

            string tooltip_onClick = "";
            string tooltip_whileBeingClicked = "";
            string tooltip_onSelect = "";
            string tooltip_onUnSelect = "";

            if (EditorGUILayout.BeginFadeGroup(showModuleSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 2;
                ModuleContainerList("On Click", tooltip_onClick, myTarget.onClickModuleContainers, onClickModuleContainers);
                GUILayout.Space(10);
                ModuleContainerList("While being clicked", tooltip_whileBeingClicked, myTarget.onPressModuleContainers, onPressModuleContainers);
                GUILayout.Space(10);
                ModuleContainerList("On Select modules", tooltip_onSelect, myTarget.onSelectModuleContainers, onSelectModuleContainers);
                GUILayout.Space(10);
                ModuleContainerList("On Un-Select modules", tooltip_onUnSelect, myTarget.unSelectModuleContainers, unSelectModuleContainers);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
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


        private void MainSettings()
        {
            EditorGUI.indentLevel = 0;

            AutoSelectionSettings();
        }

        private void AutoSelectionSettings()
        {
            EditorGUILayout.PropertyField(autoFocusOnStart, new GUIContent("Auto focus"));
            EditorGUILayout.PropertyField(autoFocusFirstItem, new GUIContent("Auto select first item"));
        }

        private void VisualSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(UseStyle, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent showVisualSettingsContent = new GUIContent("Style", "Controls child button visual.");
            showChildVisualSettings.target = EditorGUILayout.Foldout(showChildVisualSettings.target, showVisualSettingsContent, true, foldOutStyle);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showChildVisualSettings.faded))
            {
                GUILayout.Space(5);
                NormalItemSettings();
                GUILayout.Space(2.5f);
                SelectedItemSettings();
                GUILayout.Space(2.5f);
                PressedItemSettings();
                GUILayout.Space(2.5f);
                DisabledItemSettings();

                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void DebugSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent showDebugContent = new GUIContent("Debug", "");
            showDebugSettingsInEditor.target = EditorGUILayout.Foldout(showDebugSettingsInEditor.target, showDebugContent, true, foldOutStyle);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDebugSettingsInEditor.faded))
            {
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(selectedItem);
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        void NormalItemSettings()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useNormalItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showNormalItemSettings.target = EditorGUILayout.Foldout(showNormalItemSettings.target, "Normal", true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showNormalItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                HorizontalField(normalTextSize, "Text Size", "", FieldSize.extraLarge);
                HorizontalField(normalItemFontMaterial, "Text Material", "", FieldSize.extraLarge);
                HorizontalField(normalItemBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);
                GUILayout.Space(spaceAtTheBottomOfABox);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void SelectedItemSettings()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useSelectedItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent content = new GUIContent("Selected", "Mouse hover or selected in a list ready to be clicked");
            showSelectedItemSettings.target = EditorGUILayout.Foldout(showSelectedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSelectedItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                HorizontalField(selectedItemFontSize, "Text Size", "", FieldSize.extraLarge); ;
                HorizontalField(selectedItemFontMaterial, "Text Material", "", FieldSize.extraLarge);
                HorizontalField(selectedItemBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);

                GUILayout.Space(spaceAtTheBottomOfABox);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void PressedItemSettings()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(usePressedItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent content = new GUIContent("Pressed", "When click/tocuh is pressed down or for limited time after click");
            showPressedItemSettings.target = EditorGUILayout.Foldout(showPressedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showPressedItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                HorizontalField(pressedItemFontSize, "Text Size", "", FieldSize.extraLarge); ;
                HorizontalField(pressedItemFontMaterial, "Text Material", "", FieldSize.extraLarge);
                HorizontalField(pressedItemBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);
                HorizontalField(holdPressedVisualFor, "Hold visual for", "", FieldSize.extraLarge);
                GUILayout.Space(spaceAtTheBottomOfABox);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void DisabledItemSettings()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useDisabledItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent content = new GUIContent("Disabled", "Style when button isn't interactable");
            showDisabledItemSettings.target = EditorGUILayout.Foldout(showDisabledItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDisabledItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                HorizontalField(disabledItemFontSize, "Text Size", "", FieldSize.extraLarge);
                HorizontalField(disabledItemFontMaterial, "Text Material", "", FieldSize.extraLarge);
                HorizontalField(disabledItemBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);
                GUILayout.Space(spaceAtTheBottomOfABox);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }





        private void InputSettings()
        {
            string buttonText = "Add input system";
            if (!myTarget.gameObject.GetComponent<MText_InputSystemController>())
                EditorGUILayout.HelpBox("No Input system script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.", MessageType.Info);
#if ENABLE_INPUT_SYSTEM
            else if (!myTarget.gameObject.GetComponent<PlayerInput>())
                EditorGUILayout.HelpBox("No PlayerInput script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.", MessageType.Info);
#endif
            else
                buttonText = "Update input system";

            if (GUILayout.Button(new GUIContent(buttonText, "This adds InputSystemController controller scripts and updates the methods.")))
            {
                if (!myTarget.gameObject.GetComponent<MText_InputSystemController>())
                    myTarget.gameObject.AddComponent<MText_InputSystemController>();

                //#if ENABLE_LEGACY_INPUT_MANAGER
                if (myTarget.gameObject.GetComponent<MText_InputSystemController>().upAxisEvent == null)
                    myTarget.gameObject.GetComponent<MText_InputSystemController>().upAxisEvent = new UnityEngine.Events.UnityEvent();

                if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().upAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>(), "ScrollUp"))
                    UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().upAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>().ScrollUp);




                if (myTarget.gameObject.GetComponent<MText_InputSystemController>().downAxisEvent == null)
                    myTarget.gameObject.GetComponent<MText_InputSystemController>().downAxisEvent = new UnityEngine.Events.UnityEvent();
                if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().downAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>(), "ScrollDown"))
                    UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().downAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>().ScrollDown);


                if (myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent == null)
                    myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent = new UnityEngine.Events.UnityEvent();
                if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>(), "ScrollLeft"))
                    UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>().ScrollLeft);

                if (myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent == null)
                    myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent = new UnityEngine.Events.UnityEvent();
                if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>(), "ScrollRight"))
                    UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent, myTarget.gameObject.GetComponent<MText_UI_List>().ScrollRight);
                //#endif

                if (myTarget.gameObject.GetComponent<MText_InputSystemController>().submitEvent == null)
                    myTarget.gameObject.GetComponent<MText_InputSystemController>().submitEvent = new UnityEngine.Events.UnityEvent();
                if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().submitEvent, myTarget.gameObject.GetComponent<MText_UI_List>(), "PressSelectedItem"))
                    UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().submitEvent, myTarget.gameObject.GetComponent<MText_UI_List>().PressSelectedItem);

#if ENABLE_INPUT_SYSTEM
                if (!myTarget.gameObject.GetComponent<PlayerInput>())
                    myTarget.gameObject.AddComponent<PlayerInput>();

                if (settings)
                    myTarget.gameObject.GetComponent<PlayerInput>().actions = settings.inputActionAsset;
#endif
            }
#if ENABLE_INPUT_SYSTEM
            if (myTarget.gameObject.GetComponent<PlayerInput>())
                if (!myTarget.gameObject.GetComponent<PlayerInput>().actions)
                    EditorGUILayout.HelpBox("Couldnt find InputActionAsset. Please attach 3D Text UI Control.", MessageType.Info);
#endif
        }

        private bool CheckIfContains(UnityEvent myEvent, object target, string targetMethodName)
        {
            for (int i = 0; i < myEvent.GetPersistentEventCount(); i++)
            {
                if (myEvent.GetPersistentTarget(i) == (object)target)
                {
                    if (myEvent.GetPersistentMethodName(i) == targetMethodName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void GetReferences()
        {
            GetAnimBoolReferences();

            autoFocusOnStart = soTarget.FindProperty("autoFocusOnStart");
            autoFocusFirstItem = soTarget.FindProperty("autoFocusFirstItem");

            GetStyleReferences();

            useModules = soTarget.FindProperty("useModules");
            ignoreChildModules = soTarget.FindProperty("ignoreChildModules");

            ignoreChildUnSelectModuleContainers = soTarget.FindProperty("ignoreChildUnSelectModuleContainers");
            applyUnSelectModuleContainers = soTarget.FindProperty("applyUnSelectModuleContainers");
            unSelectModuleContainers = soTarget.FindProperty("unSelectModuleContainers");

            ignoreChildOnSelectModuleContainers = soTarget.FindProperty("ignoreChildOnSelectModuleContainers");
            applyOnSelectModuleContainers = soTarget.FindProperty("applyOnSelectModuleContainers");
            onSelectModuleContainers = soTarget.FindProperty("onSelectModuleContainers");

            ignoreChildOnPressModuleContainers = soTarget.FindProperty("ignoreChildOnPressModuleContainers");
            applyOnPressModuleContainers = soTarget.FindProperty("applyOnPressModuleContainers");
            onPressModuleContainers = soTarget.FindProperty("onPressModuleContainers");

            ignoreChildOnClickModuleContainers = soTarget.FindProperty("ignoreChildOnClickModuleContainers");
            applyOnClickModuleContainers = soTarget.FindProperty("applyOnClickModuleContainers");
            onClickModuleContainers = soTarget.FindProperty("onClickModuleContainers");

            selectedItem = soTarget.FindProperty("selectedItem");
        }

        private void GetStyleReferences()
        {
            UseStyle = soTarget.FindProperty("_useStyle");

            useNormalItemVisual = soTarget.FindProperty("_useNormalItemVisual");
            normalTextSize = soTarget.FindProperty("_normalTextSize");
            normalItemFontMaterial = soTarget.FindProperty("_normalTextMaterial");
            normalItemBackgroundMaterial = soTarget.FindProperty("_normalBackgroundMaterial");

            useSelectedItemVisual = soTarget.FindProperty("_useSelectedItemVisual");
            selectedItemFontSize = soTarget.FindProperty("_selectedTextSize");
            selectedItemFontMaterial = soTarget.FindProperty("_selectedTextMaterial");
            selectedItemBackgroundMaterial = soTarget.FindProperty("_selectedBackgroundMaterial");

            usePressedItemVisual = soTarget.FindProperty("_usePressedItemVisual");
            pressedItemFontSize = soTarget.FindProperty("_pressedTextSize");
            pressedItemFontMaterial = soTarget.FindProperty("_pressedTextMaterial");
            pressedItemBackgroundMaterial = soTarget.FindProperty("_pressedBackgroundMaterial");
            holdPressedVisualFor = soTarget.FindProperty("holdPressedVisualFor");

            useDisabledItemVisual = soTarget.FindProperty("_useDisabledItemVisual");
            disabledItemFontSize = soTarget.FindProperty("_disabledTextSize");
            disabledItemFontMaterial = soTarget.FindProperty("_disabledTextMaterial");
            disabledItemBackgroundMaterial = soTarget.FindProperty("_disabledBackgroundMaterial");
        }

        private void GetAnimBoolReferences()
        {
            showLayoutSettingsInEditor = new AnimBool(false);
            showLayoutSettingsInEditor.valueChanged.AddListener(Repaint);
            showModuleSettingsInEditor = new AnimBool(false);
            showModuleSettingsInEditor.valueChanged.AddListener(Repaint);
            showDebugSettingsInEditor = new AnimBool(false);
            showDebugSettingsInEditor.valueChanged.AddListener(Repaint);
            showDisabledItemSettings = new AnimBool(false);
            showDisabledItemSettings.valueChanged.AddListener(Repaint);
            showPressedItemSettings = new AnimBool(false);
            showPressedItemSettings.valueChanged.AddListener(Repaint);
            showSelectedItemSettings = new AnimBool(false);
            showSelectedItemSettings.valueChanged.AddListener(Repaint);
            showNormalItemSettings = new AnimBool(false);
            showNormalItemSettings.valueChanged.AddListener(Repaint);
            showChildVisualSettings = new AnimBool(false);
            showChildVisualSettings.valueChanged.AddListener(Repaint);
        }

        private void GenerateStyle()
        {
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 1);
            }
        }

        private void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        private void HorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal)
        {
            float myMaxWidth;
            //not to self: it's ternary operator not tarnary operator. Stop mistyping
            if (settings)
                myMaxWidth = fieldSize == FieldSize.small ? settings.smallHorizontalFieldSize : fieldSize == FieldSize.normal ? settings.normalltHorizontalFieldSize : fieldSize == FieldSize.large ? settings.largeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? settings.extraLargeHorizontalFieldSize : settings.normalltHorizontalFieldSize;
            else
                myMaxWidth = fieldSize == FieldSize.small ? defaultSmallHorizontalFieldSize : fieldSize == FieldSize.normal ? defaultNormalltHorizontalFieldSize : fieldSize == FieldSize.large ? defaultLargeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? defaultExtraLargeHorizontalFieldSize : settings.normalltHorizontalFieldSize;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent(label, tooltip);
            EditorGUILayout.LabelField(gUIContent, GUILayout.MaxWidth(myMaxWidth));
            EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
        }
    }
}
