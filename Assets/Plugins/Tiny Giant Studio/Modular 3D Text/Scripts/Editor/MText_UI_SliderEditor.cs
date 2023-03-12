using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using MText.EditorHelper;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MText
{
    [CustomEditor(typeof(MText_UI_Slider))]
    public class MText_UI_SliderEditor : Editor
    {
        public MText_Settings settings;

        readonly float defaultSmallHorizontalFieldSize = 72.5f;
        readonly float defaultNormalltHorizontalFieldSize = 100;
        readonly float defaultLargeHorizontalFieldSize = 120f;
        readonly float defaultExtraLargeHorizontalFieldSize = 150f;


        MText_UI_Slider myTarget;
        SerializedObject soTarget;

        SerializedProperty autoFocusOnGameStart;
        SerializedProperty interactable;

        SerializedProperty minValue;
        SerializedProperty maxValue;
        SerializedProperty handle;
        SerializedProperty background;
        SerializedProperty backgroundSize;

        SerializedProperty keyStep;

        SerializedProperty handleGraphic;
        SerializedProperty progressBar;
        SerializedProperty selectedHandleMat;
        SerializedProperty unSelectedHandleMat;
        SerializedProperty clickedHandleMat;
        SerializedProperty disabledHandleMat;

        SerializedProperty useEvents;
        SerializedProperty onValueChanged;
        SerializedProperty sliderDragEnded;

        SerializedProperty useValueRangeEvents;
        SerializedProperty usePercentage;
        SerializedProperty valueRangeEvents;

        float value;

        bool showKeyboardSettings = false;
        bool showVisualSettings = false;
        bool showEventsSettings = false;
        bool showValueRangeSettings = false;


        readonly string[] directionOptions = new[] { "Left to Right", "Right to Left" };
        enum FieldSize
        {
            small,
            normal,
            large,
            extraLarge
        }

        GUIStyle foldOutStyle;



        void OnEnable()
        {
            myTarget = (MText_UI_Slider)target;
            soTarget = new SerializedObject(target);

            autoFocusOnGameStart = soTarget.FindProperty("autoFocusOnGameStart");
            interactable = soTarget.FindProperty("interactable");

            minValue = soTarget.FindProperty("minValue");
            maxValue = soTarget.FindProperty("maxValue");
            handle = soTarget.FindProperty("handle");
            background = soTarget.FindProperty("background");
            backgroundSize = soTarget.FindProperty("backgroundSize");

            keyStep = soTarget.FindProperty("keyStep");

            onValueChanged = soTarget.FindProperty("onValueChanged");
            sliderDragEnded = soTarget.FindProperty("sliderDragEnded");
            useEvents = soTarget.FindProperty("useEvents");

            handleGraphic = soTarget.FindProperty("handleGraphic");
            progressBar = soTarget.FindProperty("progressBar");
            selectedHandleMat = soTarget.FindProperty("selectedHandleMat");
            unSelectedHandleMat = soTarget.FindProperty("unSelectedHandleMat");
            clickedHandleMat = soTarget.FindProperty("clickedHandleMat");
            disabledHandleMat = soTarget.FindProperty("disabledHandleMat");

            useValueRangeEvents = soTarget.FindProperty("useValueRangeEvents");
            usePercentage = soTarget.FindProperty("usePercentage");
            valueRangeEvents = soTarget.FindProperty("valueRangeEvents");


            showKeyboardSettings = myTarget.showKeyboardSettings;
            showVisualSettings = myTarget.showVisualSettings;
            showEventsSettings = myTarget.showEventsSettings;
            showValueRangeSettings = myTarget.showValueRangeSettings;

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
            KeyboardSettings();
            EventSettings();
            ValueRanges();

            SaveInspectorLayoutSettings();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                ApplyModifiedValuesToHandleAndBackground();
                EditorUtility.SetDirty(myTarget);
            }
        }


        void MainSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            DrawUILine(Color.grey, 1, 2);
            HorizontalField(autoFocusOnGameStart, "Auto Focus", "Selects on Awake() \nSelected items can be controlled by keyboard.\nIf it's in a list, this is controlled by list", FieldSize.small);
            HorizontalField(interactable, "Interactable", "", FieldSize.small);


            GUILayout.Space(15);

            EditorGUILayout.PropertyField(minValue);
            EditorGUILayout.PropertyField(maxValue);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Value", GUILayout.MaxWidth(100));
            value = EditorGUILayout.Slider(myTarget.Value, myTarget.minValue, myTarget.maxValue);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            EditorGUILayout.PropertyField(handle);
            EditorGUILayout.PropertyField(progressBar);

            EditorGUILayout.PropertyField(background);
            EditorGUILayout.PropertyField(backgroundSize);
            GUILayout.Space(5);

            myTarget.directionChoice = EditorGUILayout.Popup(myTarget.directionChoice, directionOptions);

            //if (!myTarget.handle)
            //{
            //    EditorGUILayout.HelpBox("A slider handle script is required. Even if it's disabled", MessageType.Warning);
            //}

            GUILayout.EndVertical();
        }

        private void VisualSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showVisualSettings = EditorGUILayout.Foldout(showVisualSettings, "Visuals", true, foldOutStyle);
            GUILayout.EndVertical();

            if (showVisualSettings)
            {
                EditorGUI.indentLevel = 0;

                EditorGUILayout.PropertyField(handleGraphic);
                EditorGUILayout.PropertyField(selectedHandleMat);
                EditorGUILayout.PropertyField(unSelectedHandleMat);
                EditorGUILayout.PropertyField(clickedHandleMat);
                EditorGUILayout.PropertyField(disabledHandleMat);

                GUILayout.Space(5);
            }
            if (!Selection.activeTransform)
            {
                showVisualSettings = false;
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        void KeyboardSettings()
        {
            GUILayout.BeginVertical("Box");

            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showKeyboardSettings = EditorGUILayout.Foldout(showKeyboardSettings, "Keyboard Control", true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (showKeyboardSettings)
            {
                EditorGUILayout.PropertyField(keyStep);

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

#if ENABLE_INPUT_SYSTEM
#else
                    myTarget.gameObject.GetComponent<MText_InputSystemController>().tickRate = 0.0001f;
#endif
                    if (myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent == null)
                        myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent = new UnityEngine.Events.UnityEvent();
                    if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent, myTarget.gameObject.GetComponent<MText_UI_Slider>(), "DecreaseValue"))
                        UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().leftAxisEvent, myTarget.gameObject.GetComponent<MText_UI_Slider>().DecreaseValue);


                    if (myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent == null)
                        myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent = new UnityEngine.Events.UnityEvent();
                    if (!CheckIfContains(myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent, myTarget.gameObject.GetComponent<MText_UI_Slider>(), "IncreaseValue"))
                        UnityEventTools.AddPersistentListener(myTarget.gameObject.GetComponent<MText_InputSystemController>().rightAxisEvent, myTarget.gameObject.GetComponent<MText_UI_Slider>().IncreaseValue);

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
            if (!Selection.activeTransform)
            {
                showKeyboardSettings = false;
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
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
        void EventSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useEvents, GUIContent.none, GUILayout.MaxWidth(25));
            showEventsSettings = EditorGUILayout.Foldout(showEventsSettings, "Unit Events", true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (showEventsSettings)
            {
                EditorGUILayout.PropertyField(onValueChanged);
                EditorGUILayout.PropertyField(sliderDragEnded);

                GUILayout.Space(5);
            }
            if (!Selection.activeTransform)
            {
                showEventsSettings = false;
            }
            GUILayout.EndVertical();
        }


        void ValueRanges()
        {
            GUILayout.Space(10);
            GUIContent tabName = new GUIContent("Value Range Events", "Stuff to happen when slider value enters a specific range.\nChecks value in top to down order. If you have two ranges that can be fulfilled at the same time, the first one gets called");

            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useValueRangeEvents, GUIContent.none, GUILayout.MaxWidth(25));
            showValueRangeSettings = EditorGUILayout.Foldout(showValueRangeSettings, tabName, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            Color color = GUI.backgroundColor;

            if (showValueRangeSettings)
            {
                if (settings)
                    GUI.backgroundColor = settings.tabSelectedColor;
                else
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);

                DrawUILine(Color.grey, 1, 2);
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(usePercentage);
                for (int i = 0; i < myTarget.valueRangeEvents.Count; i++)
                {

                    GUILayout.BeginVertical("Box");
                    EditorGUI.indentLevel = 0;
                    GUILayout.Space(5);

                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("min"), new GUIContent("Minimum value"));
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("max"), new GUIContent("Maximum value"));

                    if (myTarget.usePercentage && (myTarget.valueRangeEvents[i].min > 100 || myTarget.valueRangeEvents[i].max > 100))
                        EditorGUILayout.HelpBox("A range value is greater than 100 percant. Uncheck Use Percentage if you want to use direct values", MessageType.Warning);

                    if (myTarget.valueRangeEvents[i].min > myTarget.valueRangeEvents[i].max)
                        EditorGUILayout.HelpBox("Minimum value is greater than maximum", MessageType.Warning);


                    GUILayout.Space(2);

                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("icon"));
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("oneTimeEvents"));
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("repeatEvents"), new GUIContent("Gets called everytime slider value is changed at this range", ""));

                    GUI.contentColor = Color.black;
                    if (GUILayout.Button("X", GUILayout.MaxHeight(20)))
                    {
                        myTarget.valueRangeEvents.RemoveAt(i);
                    }
                    GUILayout.Space(5);

                    GUILayout.EndVertical();

                    GUILayout.Space(10);
                }

                if (GUILayout.Button("+", GUILayout.MinHeight(20)))
                {
                    myTarget.NewValueRange();
                }
            }
            GUI.backgroundColor = color;

            if (!Selection.activeTransform)
            {
                showValueRangeSettings = false;
            }
            GUILayout.EndVertical();
        }




        void ApplyModifiedValuesToHandleAndBackground()
        {
            if (value != myTarget.Value)
                myTarget.Value = value;

            if (myTarget.background)
                myTarget.UpdateBackgroundSize();
            //myTarget.background.localScale = new Vector3(myTarget.backgroundSize, myTarget.background.localScale.y, myTarget.background.localScale.z);

            if (myTarget)
                myTarget.UpdateGraphic();

            if (myTarget.interactable)
                myTarget.UnSelectedVisual();
            else
                myTarget.DisabledVisual();
        }

        void SaveInspectorLayoutSettings()
        {
            myTarget.showKeyboardSettings = showKeyboardSettings;
            myTarget.showVisualSettings = showVisualSettings;
            myTarget.showEventsSettings = showEventsSettings;
            myTarget.showValueRangeSettings = showValueRangeSettings;
        }


        void HorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal)
        {
            float myMaxWidth;
            //not to self: it's ternary operator not tarnary operator. Stop mistaking
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

        void DrawUILine(Color color, int thickness = 1, int padding = 2)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        void GenerateStyle()
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
    }
}