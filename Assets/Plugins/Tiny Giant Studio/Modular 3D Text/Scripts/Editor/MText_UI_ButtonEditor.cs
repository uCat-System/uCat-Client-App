using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(MText_UI_Button))]
    public class MText_UI_ButtonEditor : Editor
    {
        public MText_Settings settings;

        MText_UI_Button myTarget;
        SerializedObject soTarget;

        bool showEvents = false;
        SerializedProperty onClickEvents;
        SerializedProperty whileBeingClickedEvents;
        SerializedProperty onSelectEvents;
        SerializedProperty onUnselectEvents;

        SerializedProperty interactable;
        SerializedProperty interactableByMouse;

        SerializedProperty text;
        SerializedProperty background;

        SerializedProperty useStyles;

        SerializedProperty normalTextSize;
        SerializedProperty normalTextMaterial;
        SerializedProperty normalBackgroundMaterial;

        SerializedProperty useSelectedVisual;
        SerializedProperty selectedTextSize;
        SerializedProperty selectedTextMaterial;
        SerializedProperty selectedBackgroundMaterial;

        SerializedProperty usePressedVisual;
        SerializedProperty pressedTextSize;
        SerializedProperty pressedTextMaterial;
        SerializedProperty pressedBackgroundMaterial;
        SerializedProperty holdPressedVisualFor;


        SerializedProperty useDisabledVisual;
        SerializedProperty disabledTextSize;
        SerializedProperty disabledTextMaterial;
        SerializedProperty disabledBackgroundMaterial;

        //bool showModuleSettings = false;
        SerializedProperty useModules;

        SerializedProperty unSelectModuleContainers;

        SerializedProperty onSelectModuleContainers;

        SerializedProperty onPressModuleContainers;

        SerializedProperty onClickModuleContainers;

        SerializedProperty hideOverwrittenVariablesFromInspector;


        private static GUIStyle foldOutStyle = null;


        private static Color toggledOnButtonColor = Color.white;
        private static Color toggledOffButtonColor = Color.gray;
        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);

        AnimBool showEventSettings;
        AnimBool showModuleSettings;
        AnimBool showAdvancedSettings;

        AnimBool showDisabledItemSettings;
        AnimBool showPressedItemSettings;
        AnimBool showSelectedItemSettings;
        AnimBool showNormalItemSettings;
        AnimBool showVisualSettings;


        void OnEnable()
        {
            myTarget = (MText_UI_Button)target;
            soTarget = new SerializedObject(target);

            showModuleSettings = new AnimBool(false);
            showModuleSettings.valueChanged.AddListener(Repaint);
            showAdvancedSettings = new AnimBool(false);
            showAdvancedSettings.valueChanged.AddListener(Repaint);


            onSelectEvents = soTarget.FindProperty("onSelect");
            onUnselectEvents = soTarget.FindProperty("onUnselect");
            whileBeingClickedEvents = soTarget.FindProperty("whileBeingClicked");
            onClickEvents = soTarget.FindProperty("onClick");

            interactable = soTarget.FindProperty("interactable");
            interactableByMouse = soTarget.FindProperty("interactableByMouse");

            text = soTarget.FindProperty("_text");
            background = soTarget.FindProperty("_background");


            useStyles = soTarget.FindProperty("useStyles");

            normalTextSize = soTarget.FindProperty("_normalTextSize");
            normalTextMaterial = soTarget.FindProperty("_normalTextMaterial");
            normalBackgroundMaterial = soTarget.FindProperty("_normalBackgroundMaterial");

            useSelectedVisual = soTarget.FindProperty("useSelectedVisual");
            selectedTextSize = soTarget.FindProperty("_selectedTextSize");
            selectedTextMaterial = soTarget.FindProperty("_selectedTextMaterial");
            selectedBackgroundMaterial = soTarget.FindProperty("_selectedBackgroundMaterial");

            usePressedVisual = soTarget.FindProperty("usePressedVisual");
            pressedTextSize = soTarget.FindProperty("_pressedTextSize");
            pressedTextMaterial = soTarget.FindProperty("_pressedTextMaterial");
            pressedBackgroundMaterial = soTarget.FindProperty("_pressedBackgroundMaterial");
            holdPressedVisualFor = soTarget.FindProperty("holdPressedVisualFor");

            useDisabledVisual = soTarget.FindProperty("_useDisabledVisual");
            disabledTextSize = soTarget.FindProperty("_disabledTextSize");
            disabledTextMaterial = soTarget.FindProperty("_disabledTextMaterial");
            disabledBackgroundMaterial = soTarget.FindProperty("_disabledBackgroundMaterial");



            useModules = soTarget.FindProperty("useModules");

            unSelectModuleContainers = soTarget.FindProperty("unSelectModuleContainers");
            onSelectModuleContainers = soTarget.FindProperty("onSelectModuleContainers");
            onPressModuleContainers = soTarget.FindProperty("onPressModuleContainers");
            onClickModuleContainers = soTarget.FindProperty("onClickModuleContainers");

            hideOverwrittenVariablesFromInspector = soTarget.FindProperty("hideOverwrittenVariablesFromInspector");

            showEvents = myTarget.showEvents;

            GetAnimBoolReferences();
        }

        private void GetAnimBoolReferences()
        {
            showEventSettings = new AnimBool(false);
            showEventSettings.valueChanged.AddListener(Repaint);  
            
            showVisualSettings = new AnimBool(false);
            showVisualSettings.valueChanged.AddListener(Repaint);

            showNormalItemSettings = new AnimBool(false);
            showNormalItemSettings.valueChanged.AddListener(Repaint);

            showSelectedItemSettings = new AnimBool(false);
            showSelectedItemSettings.valueChanged.AddListener(Repaint);

            showPressedItemSettings = new AnimBool(false);
            showPressedItemSettings.valueChanged.AddListener(Repaint);

            showDisabledItemSettings = new AnimBool(false);
            showDisabledItemSettings.valueChanged.AddListener(Repaint);
        }


        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            Warning();
            GUILayout.Space(10);
            MainSettings();
            GUILayout.Space(10);

            Styles();
            GUILayout.Space(6);
            Events();
            GUILayout.Space(6);
            ModuleSettings();
            GUILayout.Space(6);
            AdvancedSettings();


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();

                myTarget.UpdateStyle();

                EditorUtility.SetDirty(myTarget);
            }
        }
        void Warning()
        {
            if (myTarget.ApplyNormalStyle().Item1 || myTarget.ApplyOnSelectStyle().Item1 || myTarget.ApplyPressedStyle().Item1 || myTarget.ApplyDisabledStyle().Item1)
                EditorGUILayout.HelpBox("Some values are overwritten by parent list.", MessageType.Info);
        }


        void MainSettings()
        {
            EditorGUILayout.PropertyField(text);
            EditorGUILayout.PropertyField(background);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(interactable);
            if (myTarget.interactable)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(interactableByMouse, new GUIContent("By mouse/touch"));
            }
        }

        void Styles()
        {
            if (myTarget.hideOverwrittenVariablesFromInspector && myTarget.ApplyNormalStyle().Item1 && myTarget.ApplyOnSelectStyle().Item1 && myTarget.ApplyPressedStyle().Item1 && myTarget.ApplyDisabledStyle().Item1)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useStyles, GUIContent.none, GUILayout.MaxWidth(25));
            showVisualSettings.target = EditorGUILayout.Foldout(showVisualSettings.target, "Style", true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showVisualSettings.faded))
            {
                NormalStyle();
                SelectedStyle();
                PressedItemSettings();
                DisabledtStyle();
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }


        void NormalStyle()
        {
            if (myTarget.ApplyNormalStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;



            EditorGUI.indentLevel = 2;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showNormalItemSettings.target = EditorGUILayout.Foldout(showNormalItemSettings.target, "Normal", true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showNormalItemSettings.faded))
            {
                if (myTarget.ApplyNormalStyle().Item1)
                {
                    EditorGUILayout.HelpBox("Normal style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }

                MText_Editor_Methods.HorizontalField(normalTextSize, "Text Size", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(normalTextMaterial, "Text Material", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(normalBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);

            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void SelectedStyle()
        {
            if (myTarget.ApplyOnSelectStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Selected", "Mouse hover or selected in a list ready to be clicked");
            EditorGUILayout.PropertyField(useSelectedVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showSelectedItemSettings.target = EditorGUILayout.Foldout(showSelectedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSelectedItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                if (myTarget.ApplyOnSelectStyle().Item1)
                {
                    EditorGUILayout.HelpBox("On select style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }

                MText_Editor_Methods.HorizontalField(selectedTextSize, "Text Size", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(selectedTextMaterial, "Text Material", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(selectedBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);

            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void PressedItemSettings()
        {
            if (myTarget.ApplyPressedStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;


            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Pressed", "When click/tocuh is pressed down or for limited time after click");
            EditorGUILayout.PropertyField(usePressedVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showPressedItemSettings.target = EditorGUILayout.Foldout(showPressedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showPressedItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                if (myTarget.ApplyPressedStyle().Item1)
                {
                    EditorGUILayout.HelpBox("Pressed style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }

                MText_Editor_Methods.HorizontalField(pressedTextSize, "Text Size", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(pressedTextMaterial, "Text Material", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(pressedBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(holdPressedVisualFor, "Hold pressed for", "How long this visual lasts. This is not for mouse/touch click", FieldSize.extraLarge);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void DisabledtStyle()
        {
            if (myTarget.ApplyDisabledStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;



            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Disabled", "Style when button isn't interactable");
            EditorGUILayout.PropertyField(useDisabledVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showDisabledItemSettings.target = EditorGUILayout.Foldout(showDisabledItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDisabledItemSettings.faded))
            {
                EditorGUI.indentLevel = 2;
                if (myTarget.ApplyDisabledStyle().Item1)
                {
                    EditorGUILayout.HelpBox("Disabled style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }

                MText_Editor_Methods.HorizontalField(disabledTextSize, "Text Size", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(disabledTextMaterial, "Text Material", "", FieldSize.extraLarge);
                MText_Editor_Methods.HorizontalField(disabledBackgroundMaterial, "Background Material", "", FieldSize.extraLarge);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void Events()
        {

            EditorGUI.indentLevel = 2;
            GUILayout.BeginVertical("Box");

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Unity Events",
                           "On Click: \nMouse click/Touch finished or list pressed enter " +
                           "\n\nWhile being Clicked: \nWhen click is pressed down" +
                           "\n\nOn Select: \nMouse hover or selected in a list ready to be clicked" +
                           "\n\nOn Unselect: \nMouse/Touch moved away or list unselected");

            showEventSettings.target = EditorGUILayout.Foldout(showEventSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showEventSettings.faded))
            {
                EditorGUILayout.PropertyField(onClickEvents);
                EditorGUILayout.PropertyField(whileBeingClickedEvents);
                EditorGUILayout.PropertyField(onSelectEvents);
                EditorGUILayout.PropertyField(onUnselectEvents);
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
            showModuleSettings.target = EditorGUILayout.Foldout(showModuleSettings.target, new GUIContent("Modules", "Modules provide an easy way animate characters. \n They are called in two events. \n1. When new characters are added to text. \n2. When a character is removed from the text. \nThis change can be done by code or ui or anything. The text string is a property."), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            string tooltip_onClick = "";
            string tooltip_whileBeingClicked = "";
            string tooltip_onSelect = "";
            string tooltip_onUnSelect = "";

            if (EditorGUILayout.BeginFadeGroup(showModuleSettings.faded))
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

        private void AdvancedSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showAdvancedSettings.target = EditorGUILayout.Foldout(showAdvancedSettings.target, new GUIContent("Advanced settings", ""), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showAdvancedSettings.faded))
            {
                EditorGUI.indentLevel = 1;
                MText_Editor_Methods.HorizontalField(hideOverwrittenVariablesFromInspector, "Hide overwritten values", "Buttons under list sometimes have styles overwritten. This hides these variables", FieldSize.extraLarge);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }


        void GenerateStyle()
        {
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout);
                foldOutStyle.overflow = new RectOffset(-10, 0, 3, 0);
                foldOutStyle.padding = new RectOffset(15, 0, -3, 0);
                foldOutStyle.fontStyle = FontStyle.Bold;
                foldOutStyle.onNormal.textColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 1);
            }
        }

        void DrawUILine(Color color, int thickness = 1, int padding = 0)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}