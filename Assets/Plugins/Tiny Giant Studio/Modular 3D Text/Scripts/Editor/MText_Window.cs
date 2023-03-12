using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;

//using TinyGiantStudio.Editor;
using MText.EditorHelper;
using MText.FontCreation;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// Created by Ferdowsur Asif @ Tiny Giant Studio
namespace MText
{
    public class MText_Window : EditorWindow
    {
        SerializedObject soTarget;

        private readonly string version = "3.3.1";
        private readonly string documentationURL = "https://ferdowsur.gitbook.io/modular-3d-text/";
        private readonly string fontCreationInEditor_WrittenTutorialURL = "https://ferdowsur.gitbook.io/modular-3d-text/creating-fonts";
        private readonly string fontCreationInEditor_VideoTutorialURL = "https://youtu.be/m9JwBc-0DUA";
        private readonly string supportEmail = "asifno13@gmail.com";



        public MText_Settings settings;
        private string selectedTab = "Getting Started";
        //private bool neverStartedBefore = true;


        readonly GUIContent gettingStarted_TabContent = new GUIContent("General");
        readonly string gettingStarted_TabName = "Getting Started";

        readonly GUIContent fontCreation_TabContent = new GUIContent("Font Creation");
        readonly string fontCreation_TabName = "Font Creation";

        readonly GUIContent defaultValues_TabContent = new GUIContent("Defaults");
        readonly string defaultValues_TabName = "Settings";

        readonly GUIContent feedback_TabContent = new GUIContent("Feedback");
        readonly string feedback_TabName = "Feedback";

        readonly GUIContent utility_TabContent = new GUIContent("Utility");
        readonly string utility_TabName = "Utility";


        GUIStyle tabStyle;
        GUIStyle gridStyle;
        GUIStyle foldOutStyle;

        Texture titleTexture;


        readonly private int indent = 7;
        readonly private int labelWidth = 135;
        readonly private int spaceBetweenContents = 5;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);

        AnimBool showDefaultTextSettings;
        AnimBool showDefaultButtonSettings;
        AnimBool showDefaultListSettings;

        void OnEnable()
        {
            titleTexture = Resources.Load("Modular 3D Text/Window Title") as Texture;

            VerifyReferences();

            if (soTarget == null)
                soTarget = new SerializedObject(settings);

            showDefaultTextSettings = new AnimBool(false);
            showDefaultTextSettings.valueChanged.AddListener(Repaint);
            showDefaultButtonSettings = new AnimBool(false);
            showDefaultButtonSettings.valueChanged.AddListener(Repaint);
            showDefaultListSettings = new AnimBool(false);
            showDefaultListSettings.valueChanged.AddListener(Repaint);
        }





        [MenuItem("Tools/Tiny Giant Studio/Modular 3D Text", false, 100)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(MText_Window), false, "Modular 3D Text");
            editorWindow.minSize = new Vector2(450, 650);
            editorWindow.Show();
        }


        void VerifyReferences()
        {
            if (!settings)
                settings = MText_FindResource.VerifySettings(settings);
        }


        private void OnGUI()
        {
            if (soTarget == null)
                soTarget = new SerializedObject(settings);

            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            Title();
            Tabs();

            GUILayout.Space(10);

            if (selectedTab == gettingStarted_TabName)
                GeneralInformation();

            else if (selectedTab == fontCreation_TabName)
                FontCreation();

            else if (selectedTab == defaultValues_TabName)
                Preference();

            else if (selectedTab == feedback_TabName)
                Feedback();

            else if (selectedTab == utility_TabName)
                UtilityTab();



            GUI.backgroundColor = Color.white;

            GUILayout.Space(5f);
            DrawUILine(Color.grey, 1, 2);
            ProductInformation();
            GUILayout.Space(5f);


            if (EditorGUI.EndChangeCheck())
            {
                if (soTarget != null)
                    soTarget.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
            }
        }

        private void Title()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(titleTexture), GUILayout.Height(100), GUILayout.Width(400));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void Tabs()
        {
            Color originalWindowColor = GUI.backgroundColor;
            Color unselectedTabColor = settings ? settings.tabUnselectedColor : Color.grey;
            Color selectedTabColor = settings ? settings.tabSelectedColor : Color.white;


            //Draw tabs
            GUILayout.BeginHorizontal();

            //Getting started
            GUI.backgroundColor = selectedTab == gettingStarted_TabName ? selectedTabColor : unselectedTabColor;
            GUI.contentColor = selectedTab == gettingStarted_TabName ? Color.white : Color.gray;
            if (MidButton(gettingStarted_TabContent))
                selectedTab = gettingStarted_TabName;

            //Font creation
            GUI.backgroundColor = selectedTab == fontCreation_TabName ? selectedTabColor : unselectedTabColor;
            GUI.contentColor = selectedTab == fontCreation_TabName ? Color.white : Color.gray;
            if (MidButton(fontCreation_TabContent))
                selectedTab = fontCreation_TabName;

            //Input
            GUI.backgroundColor = selectedTab == defaultValues_TabName ? selectedTabColor : unselectedTabColor;
            GUI.contentColor = selectedTab == defaultValues_TabName ? Color.white : Color.gray;
            if (MidButton(defaultValues_TabContent))
                selectedTab = defaultValues_TabName;

            //Utility
            GUI.backgroundColor = selectedTab == utility_TabName ? selectedTabColor : unselectedTabColor;
            GUI.contentColor = selectedTab == utility_TabName ? Color.white : Color.gray;
            if (MidButton(utility_TabContent))
                selectedTab = utility_TabName;


            GUI.backgroundColor = selectedTab == feedback_TabName ? selectedTabColor : unselectedTabColor;
            GUI.contentColor = selectedTab == feedback_TabName ? Color.white : Color.gray;
            if (MidButton(feedback_TabContent))
                selectedTab = feedback_TabName;




            GUI.backgroundColor = originalWindowColor;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }






        #region First Tab: Getting Started

        private void GeneralInformation()
        {
            ////Need a note if package update is required
            //if (GUILayout.Button("Update package"))
            //{
            //    TGSImporterWindow.Open();
            //}
            GridContent("How to create 3D UI :", "Right click in your hierarchy,\n\tGameobject > 3D Object > Modular 3D Text");
            GUILayout.Space(spaceBetweenContents);
            GridContent("Documentation", "There is no better place to get started than the documentation.", "Documentation", documentationURL);
            GUILayout.Space(spaceBetweenContents);
            FontCreationTutorialLink();
            GUILayout.Space(spaceBetweenContents);
            ForumLink();
            GUILayout.Space(spaceBetweenContents);
            SupportLink();
        }





        #endregion First Tab: Getting Started



        #region Feedback Tab
        void Feedback()
        {
            ForumLink();
            GUILayout.Space(spaceBetweenContents);
            SupportLink();
            GUILayout.Space(spaceBetweenContents);
            RateAssetLink();
        }
        #endregion Feedback Tab

        #region Utility Tab
        void UtilityTab()
        {
            if (GUILayout.Button("Mark all object in scene visible in hierarchy "))
            {
                foreach (var t in FindObjectsOfType<Transform>())
                {
                    if (t.gameObject.hideFlags != HideFlags.None)
                    {
                        t.gameObject.hideFlags = HideFlags.None;
                    }
                }
            }

            if (GUILayout.Button("Hide selected objects in hierarchy "))
            {
                foreach (var t in Selection.objects)
                {
                    if (t.GetType() == typeof(GameObject))
                    {
                        t.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                EditorApplication.DirtyHierarchyWindowSorting();
            }
        }
        #endregion Utility Tab


        #region URL Methods
        private void SupportLink()
        {
            string description = "Need assistance with anything? Always happy to help. \nReachout to me at " + supportEmail;

            Color originalWindowColor = GUI.backgroundColor;
            Color boxBackgroundColor = Color.white;
            if (settings)
                boxBackgroundColor = settings.gridItemColor;
            GUI.backgroundColor = boxBackgroundColor;

            GUILayout.BeginVertical("", gridStyle);
            GUILayout.Space(5);
            GUILayout.Label("Support", EditorStyles.boldLabel);
            GUILayout.Label(description);

            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Email to", GUILayout.MinHeight(25)))
            {
                Application.OpenURL("mailto:" + supportEmail + "?subject=Modular 3D Text Support");
            }
            if (GUILayout.Button("Copy mail to clipboard", GUILayout.MinHeight(25)))
            {
                GUIUtility.systemCopyBuffer = supportEmail;
            }
            GUILayout.EndHorizontal();
            GUILayout.Label(supportEmail, EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
        private void ForumLink()
        {
            GridContent("Forum", "Join the conversation in Unity Forum. Discuss anything and everything.", "Unity Forum", "https://forum.unity.com/threads/modular-3d-text-3d-texts-for-your-3d-game.821931/");
        }
        private void RateAssetLink()
        {
            GridContent("Rate the asset", "Reviews are one of the primary methods of getting discovered in Unity Asset store. If you have some time, please rate the asset a 5 star. I would really appriciate it. \nEither way, thank you for buying the asset! You keep me motivated to continuously improve the asset.", "Rate the Asset", "https://assetstore.unity.com/packages/3d/gui/modular-3d-text-in-game-3d-ui-system-159508?aid=1011ljxWe&utm_source=aff");
        }
        private void FontCreationTutorialLink()
        {
            Color originalWindowColor = GUI.backgroundColor;
            Color boxBackgroundColor = Color.white;
            if (settings)
                boxBackgroundColor = settings.gridItemColor;
            GUI.backgroundColor = boxBackgroundColor;



            GUILayout.BeginVertical("", gridStyle);
            GUILayout.Space(5);
            GUILayout.Label("Font Tutorial", EditorStyles.boldLabel);
            GUILayout.Label("Want to give your texts an unique look? Create your own fonts.", EditorStyles.wordWrappedLabel);

            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Written", GUILayout.MinHeight(25)))
            {
                Application.OpenURL(fontCreationInEditor_WrittenTutorialURL);
            }
            if (GUILayout.Button("Video", GUILayout.MinHeight(25)))
            {
                Application.OpenURL(fontCreationInEditor_VideoTutorialURL);
            }
            GUILayout.EndHorizontal();

            //if (GUILayout.Button("In Blender (Written)", GUILayout.MinHeight(25)))
            //{
            //    Application.OpenURL("https://app.gitbook.com/@ferdowsur/s/tinygiantstudios/~/drafts/-Mac8hd9ymh3uAidv7ov/#using-blender");
            //}
            //if (GUILayout.Button("In Blender (Video)", GUILayout.MinHeight(25)))
            //{
            //    Application.OpenURL("https://youtu.be/2ixgOJ_sXtI");
            //}
            GUILayout.EndVertical();



            GUI.backgroundColor = originalWindowColor;
        }
        #endregion URL Methods



        Vector2 scrollPos;
        void Preference()
        {
            if (settings)
            {
                scrollPos =
           EditorGUILayout.BeginScrollView(scrollPos);
                TextPreference();

                ButtonPreference();

                ListPreference();



#if ENABLE_INPUT_SYSTEM
                //GUILayout.Label("Input System", EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Input Action Asset", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("inputActionAsset"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                {
                    ApplyInputActionAssetToScene();
                }
                EditorGUILayout.EndHorizontal();
#endif
                EditorGUILayout.EndScrollView();
            }

            //    EditorGUILayout.HelpBox("Under Construction. Expect a LOT more options in the future.", MessageType.Info);

        }

        private void ListPreference()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDefaultListSettings.target = EditorGUILayout.Foldout(showDefaultListSettings.target, "List", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDefaultListSettings.faded))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Normal Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListNormalTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListNormalTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Normal Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListNormalTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListNormalTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Normal Background", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListNormalBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListNormalBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(spaceBetweenContents);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Selected Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListSelectedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListSelectedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Selected Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListSelectedTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListSelectedTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Selected Backgroud", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListSelectedBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListSelectedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(spaceBetweenContents);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Pressed Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListPressedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListPressedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Pressed Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListPressedTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListPressedTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Pressed Backgroud", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListPressedBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListPressedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(spaceBetweenContents);


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Disabled Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListDisabledTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListDisabledTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Disabled Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListDisabledTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListDisabledTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Disabled Backgroud", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultListDisabledBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultListDisabledBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void ButtonPreference()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDefaultButtonSettings.target = EditorGUILayout.Foldout(showDefaultButtonSettings.target, "Button", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDefaultButtonSettings.faded))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Normal Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonNormalTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonNormalTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Normal Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonNormalTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonNormalTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Normal Background", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonNormalBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonNormalBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(spaceBetweenContents);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Selected Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonSelectedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonSelectedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Selected Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonSelectedTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonSelectedTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Selected Backgroud", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonSelectedBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonSelectedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(spaceBetweenContents);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Pressed Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonPressedTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonPressedTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Pressed Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonPressedTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonPressedTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Pressed Backgroud", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonPressedBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonPressedBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(spaceBetweenContents);


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Disabled Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonDisabledTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonDisabledTextSizeToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Disabled Text", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonDisabledTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonDisabledTextMaterialToScene();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Disabled Backgroud", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultButtonDisabledBackgroundMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                    ApplyDefaultButtonDisabledBackgroundMaterialToScene();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void TextPreference()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDefaultTextSettings.target = EditorGUILayout.Foldout(showDefaultTextSettings.target, "Text", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDefaultTextSettings.faded))
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Font", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultFont"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                {
                    ApplyDefaultFontToScene();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Text Size", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultTextSize"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                {
                    ApplyDefaultTextSizeToScene();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(indent));
                EditorGUILayout.LabelField("Text Material", GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(soTarget.FindProperty("defaultTextMaterial"), GUIContent.none);
                if (GUILayout.Button("Apply to scene", GUILayout.MaxWidth(100)))
                {
                    ApplyDefaultTextMaterialToScene();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void ProductInformation()
        {
            GUILayout.Label("Modular 3D Text \n" + version, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Created by Ferdowsur Asif @ Tiny Giant Studio.", EditorStyles.miniBoldLabel);
        }

        void ApplyDefaultFontToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultFont.name + "' font to every text active object in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                Modular3DText[] modular3DTexts = FindObjectsOfType<Modular3DText>();
                for (int i = 0; i < modular3DTexts.Length; i++)
                {
                    modular3DTexts[i].Font = settings.defaultFont;
                    EditorUtility.SetDirty(modular3DTexts[i]);
                }
            }
        }
        void ApplyDefaultTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultTextSize + "' font size to every text active object in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                Modular3DText[] modular3DTexts = FindObjectsOfType<Modular3DText>();
                for (int i = 0; i < modular3DTexts.Length; i++)
                {
                    if (modular3DTexts[i].transform.parent)
                    {
                        if (modular3DTexts[i].transform.parent.GetComponent<MText_UI_Button>())
                            continue;
                    }

                    modular3DTexts[i].FontSize = settings.defaultTextSize;
                    EditorUtility.SetDirty(modular3DTexts[i]);
                }
            }
        }
        void ApplyDefaultTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultTextMaterial.name + "' material to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                Modular3DText[] modular3DTexts = FindObjectsOfType<Modular3DText>();
                for (int i = 0; i < modular3DTexts.Length; i++)
                {
                    if (modular3DTexts[i].transform.parent)
                    {
                        if (modular3DTexts[i].transform.parent.GetComponent<MText_UI_Button>())
                        {
                            continue;
                        }

                    }
                    modular3DTexts[i].Material = settings.defaultTextMaterial;
                    EditorUtility.SetDirty(modular3DTexts[i]);
                }
            }
        }

#if ENABLE_INPUT_SYSTEM
        void ApplyInputActionAssetToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.inputActionAsset.name + "' to every player controller active object in the scene?" +
               "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
                for (int i = 0; i < playerInputs.Length; i++)
                {
                    playerInputs[i].actions = settings.inputActionAsset;
                    EditorUtility.SetDirty(playerInputs[i]);
                }
            }
        }
#endif

        #region Button
        //Button
        void ApplyDefaultButtonNormalTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].NormalTextSize = settings.defaultButtonNormalTextSize;
                    if (!Application.isPlaying)
                        buttons[i].UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonNormalTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].NormalTextMaterial = settings.defaultButtonNormalTextMaterial;
                    if (!Application.isPlaying)
                        buttons[i].UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonNormalBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].NormalBackgroundMaterial = settings.defaultButtonNormalBackgroundMaterial;
                    if (!Application.isPlaying)
                        buttons[i].UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }

        void ApplyDefaultButtonSelectedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonSelectedTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].SelectedTextSize = settings.defaultButtonSelectedTextSize;
                    if (!Application.isPlaying)
                        buttons[i].UnselectedButtonVisualUpdate();
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonSelectedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonSelectedTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].SelectedTextMaterial = settings.defaultButtonSelectedTextMaterial;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonSelectedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonSelectedBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].SelectedBackgroundMaterial = settings.defaultButtonSelectedBackgroundMaterial;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }

        void ApplyDefaultButtonPressedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonPressedTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].PressedTextSize = settings.defaultButtonPressedTextSize;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonPressedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonPressedTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].PressedTextMaterial = settings.defaultButtonPressedTextMaterial;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonPressedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonPressedBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].PressedBackgroundMaterial = settings.defaultButtonPressedBackgroundMaterial;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }

        void ApplyDefaultButtonDisabledTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledTextSize + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].DisabledTextSize = settings.defaultButtonDisabledTextSize;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonDisabledTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledTextMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].DisabledTextMaterial = settings.defaultButtonDisabledTextMaterial;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        void ApplyDefaultButtonDisabledBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledBackgroundMaterial.name + "' to every active button in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_Button[] buttons = FindObjectsOfType<MText_UI_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].DisabledBackgroundMaterial = settings.defaultButtonDisabledBackgroundMaterial;
                    EditorUtility.SetDirty(buttons[i]);
                }
            }
        }
        #endregion Button



        #region List
        void ApplyDefaultListNormalTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListNormalTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].NormalTextSize = settings.defaultListNormalTextSize;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListNormalTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonNormalTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].NormalTextMaterial = settings.defaultListNormalTextMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListNormalBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListNormalBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].NormalBackgroundMaterial = settings.defaultListNormalBackgroundMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListSelectedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListSelectedTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].SelectedTextSize = settings.defaultListSelectedTextSize;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListSelectedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListSelectedTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].SelectedTextMaterial = settings.defaultListSelectedTextMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListSelectedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListSelectedBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].SelectedBackgroundMaterial = settings.defaultListSelectedBackgroundMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListPressedTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListPressedTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].PressedTextSize = settings.defaultListPressedTextSize;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListPressedTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListPressedTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].PressedTextMaterial = settings.defaultListPressedTextMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListPressedBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListPressedBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].PressedBackgroundMaterial = settings.defaultListPressedBackgroundMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListDisabledTextSizeToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListDisabledTextSize + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].DisabledTextSize = settings.defaultListDisabledTextSize;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListDisabledTextMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultListDisabledTextMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].DisabledTextMaterial = settings.defaultListDisabledTextMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        void ApplyDefaultListDisabledBackgroundMaterialToScene()
        {
            string notice = "Are you sure you want to apply '" + settings.defaultButtonDisabledBackgroundMaterial.name + "' to every active list in the scene?" +
                "You can't press Undo for this action.";
            if (EditorUtility.DisplayDialog("Confirmation", notice, "Apply", "Do not apply"))
            {
                MText_UI_List[] lists = FindObjectsOfType<MText_UI_List>();
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].DisabledBackgroundMaterial = settings.defaultButtonDisabledBackgroundMaterial;
                    EditorUtility.SetDirty(lists[i]);
                }
            }
        }
        #endregion List











        private void FontCreation()
        {
            Color backgroundColor = GUI.backgroundColor;

            GUILayout.Space(5);
            CreateFontButton();
            GUILayout.Space(15);

            CharacterInput(backgroundColor);
            GUILayout.Space(25);

            MeshExportSettings();
            MeshSettings(backgroundColor); //vertext density, size xy, smoothing angle etc.


            GUILayout.Space(15);
            Note();
            BottomButtons(backgroundColor);
        }

        private void Note()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string tooltip = "Keep these values to 1 unless you really need to change it. Lower smoothing value will give a flat looking font and higher is smoother." +
                "\n\nYou can select the exported font and tweak settings like character/word spacing and even individual characters, including swapping their models.";
            EditorGUILayout.LabelField(new GUIContent("Hover me for info", tooltip), EditorStyles.boldLabel, GUILayout.Width(104));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //EditorGUILayout.HelpBox("Keep these values to 1 unless you really need to change it. \nSmoothing angle by default is 30 degrees (Same as blender). Lower value will give a flat looking font and higher is smoother.", MessageType.Info);
            //EditorGUILayout.HelpBox("You can select the exported font and tweak settings like character/word spacing and even individual characters, including swapping their models.", MessageType.Info);
        }

        private void BottomButtons(Color backgroundColor)
        {
            EditorGUILayout.BeginHorizontal();
            HorizontalButtonURL("Common Issues", "https://ferdowsur.gitbook.io/modular-3d-text/font-creation-troubleshoot"); //to-do make a video with the unity editor
            if (GUILayout.Button("Log characters in input", GUILayout.MinHeight(25)))
            {
                TestCharacterList();
            }
            if (GUILayout.Button("Reset to default", GUILayout.Height(25)))
            {
                bool reset = EditorUtility.DisplayDialog("Reset to default", "Reset all the font creation settings", "Confirm", "Cancel");

                if (reset)
                {
                    settings.ResetFontCreationSettings();
                    soTarget = new SerializedObject(settings);
                    EditorUtility.SetDirty(settings);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateFontButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Font", GUILayout.MinHeight(50), GUILayout.MinWidth(350)))
            {
                CreateFont();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void CharacterInput(Color normalColor)
        {
            GUI.backgroundColor = settings.tabSelectedColor;
            //GUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(soTarget.FindProperty("charInputStyle"));
            //DrawUILine(Color.gray);

            GUI.backgroundColor = normalColor;



            if (settings.charInputStyle == MText_Settings.CharInputStyle.CharacterRange)
            {
                CharacterRangeInput();

                EditorGUILayout.BeginHorizontal();
                HorizontalButtonURL("Get character list", "https://unicode-table.com/en/");
                HorizontalButtonURL("How it works", "https://youtu.be/JN_DSmdiRSI"); //to-do make a video with the unity editor
                EditorGUILayout.EndHorizontal();
            }
            else if (settings.charInputStyle == MText_Settings.CharInputStyle.UnicodeRange)
            {
                UnicodeRangeInput();

                EditorGUILayout.BeginHorizontal();
                HorizontalButtonURL("Get character list", "https://unicode-table.com/en/");
                //HorizontalButtonURL("How it works", "https://youtu.be/JN_DSmdiRSI"); //to-do make a video with the unity editor
                EditorGUILayout.EndHorizontal();
            }
            else if (settings.charInputStyle == MText_Settings.CharInputStyle.CustomCharacters)
            {
                CustomCharacters();
            }
            else if (settings.charInputStyle == MText_Settings.CharInputStyle.UnicodeSequence)
            {
                UnicodeSequence();
            }
            //else if (settings.charInputStyle == MText_Settings.CharInputStyle.CharacterSet)
            //{
            //    CharacterSet();
            //}


            //EditorGUILayout.HelpBox("Just a FYI: Having thousands of characters in a single file can cause issues.", MessageType.Info);
            //GUILayout.EndVertical();
        }
        private void MeshSettings(Color normalColor)
        {
            GUI.backgroundColor = settings.tabSelectedColor;
            //GUILayout.Label("Mesh Settings");
            //DrawUILine(Color.gray);

            GUI.backgroundColor = normalColor;

            GUILayout.BeginHorizontal();
            GUIContent vertexDensity = new GUIContent("Vertex Density", "How many verticies should be used. Has very little impact other than calculation time since vertext density is increased automatically if it fails to be created within the given amount.");
            EditorGUILayout.LabelField(vertexDensity, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("vertexDensity"), GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUIContent sizeXY = new GUIContent("Size XY", "Base font size.");
            EditorGUILayout.LabelField(sizeXY, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("sizeXY"), GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUIContent sizeZ = new GUIContent("Size Z/Depth", "Base depth");
            EditorGUILayout.LabelField(sizeZ, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("sizeZ"), GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUIContent smoothingAngle = new GUIContent("Smoothing Angle", "Any verticies with lower angle will be smooth.");
            EditorGUILayout.LabelField(smoothingAngle, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("smoothingAngle"), GUIContent.none);
            GUILayout.EndHorizontal();
        }
        private void MeshExportSettings()
        {
            GUILayout.BeginHorizontal();
            GUIContent exportStyle = new GUIContent("Export As", "Which way you want mesh to be saved as.\nWarning!: \nSaving as mesh asset means each character will be saved as separate assets in the folder. This will generate a LOT of seperate files for each character.");
            EditorGUILayout.LabelField(exportStyle, GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("meshExportStyle"), GUIContent.none);
            GUILayout.EndHorizontal();
        }

        private void CreateFont()
        {
            GameObject gameObject = new GameObject();

            bool exportAsObj = ExportAs();
            List<char> listOfChar = GetCharacterList();

            MText_CharacterGenerator fontCreator = new MText_CharacterGenerator();
            fontCreator.CreateFont(gameObject, listOfChar, settings.sizeXY, settings.sizeZ, settings.vertexDensity, settings.smoothingAngle, settings.defaultTextMaterial, exportAsObj, out byte[] fontData);

            //if (!fontCreator.WasEverythingProcessed())
            //{
            //    //EditorUtility.DisplayDialog("")
            //}

            EditorUtility.DisplayProgressBar("Creating font", "Mesh creation started", 75 / 100);
            if (gameObject.transform.childCount > 0)
            {
                if (exportAsObj)
                {
                    MText_ObjExporter objExporter = new MText_ObjExporter();
                    string prefabPath = objExporter.DoExport(gameObject, true);
                    if (string.IsNullOrEmpty(prefabPath))
                    {
                        Debug.Log("Object save failed");
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                    MText_FontExporter fontExporter = new MText_FontExporter();
                    fontExporter.CreateFontFile(prefabPath, gameObject.name, fontCreator, fontData);
                }
                else
                {
                    MText_MeshAssetExporter meshAssetExporter = new MText_MeshAssetExporter();
                    meshAssetExporter.DoExport(gameObject);
                }
            }

            EditorUtility.ClearProgressBar();
            if (Application.isPlaying) Destroy(gameObject);
            else DestroyImmediate(gameObject);
        }

        private bool ExportAs()
        {
            bool exportAsObj = true;
            if (settings.meshExportStyle != MText_Settings.MeshExportStyle.exportAsObj) exportAsObj = false;
            return exportAsObj;
        }




        private void CharacterRangeInput()
        {
            //field
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start", GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("startChar"), GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("End", GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("endChar"), GUIContent.none);
            GUILayout.EndHorizontal();

            //info
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Leave it to '!' & '~' for English.", GUILayout.Width(170));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void UnicodeRangeInput()
        {
            //field
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start", GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("startUnicode"), GUIContent.none);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("End", GUILayout.MaxWidth(120));
            EditorGUILayout.PropertyField(soTarget.FindProperty("endUnicode"), GUIContent.none);
            GUILayout.EndHorizontal();

            //info
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Leave it to 0021 & 007E for English.", GUILayout.Width(206));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void CustomCharacters()
        {
            //field
            EditorGUILayout.PropertyField(soTarget.FindProperty("customCharacters"), GUIContent.none);

            //info
            GUILayout.Label("Just type the characters you want in the font.");
        }
        private void UnicodeSequence()
        {
            //field
            EditorGUILayout.PropertyField(soTarget.FindProperty("unicodeSequence"), GUIContent.none);

            //info
            GUILayout.Label("Separate codes with ',' & create ranges with '-' .Example:\n" +
                "0021-007E, 00C0");
        }



        void TestCharacterList()
        {
            List<char> myCharacters = GetCharacterList();
            Debug.Log("Character count: " + myCharacters.Count);
            for (int i = 0; i < myCharacters.Count; i++)
            {
                Debug.Log(myCharacters[i]);
            }
        }



        private List<char> GetCharacterFromRange(char start, char end)
        {
            MText_NewFontCharacterRange characterRange = new MText_NewFontCharacterRange();
            List<char> characterList = characterRange.RetrieveCharactersList(start, end);
            return characterList;
        }

        private char ConvertCharFromUnicode(string unicode)
        {
            string s = System.Text.RegularExpressions.Regex.Unescape("\\u" + unicode);
            s.ToCharArray();
            if (s.Length > 0)
                return s[0];
            else
                return ' ';
        }



        List<char> GetCharacterList()
        {
            List<char> myChars = new List<char>();

            if (settings.charInputStyle == MText_Settings.CharInputStyle.CharacterRange)
            {
                myChars = GetCharacterFromRange(settings.startChar, settings.endChar);
            }
            else if (settings.charInputStyle == MText_Settings.CharInputStyle.UnicodeRange)
            {
                char start = ConvertCharFromUnicode(settings.startUnicode);
                char end = ConvertCharFromUnicode(settings.endUnicode);

                myChars = GetCharacterFromRange(start, end);
            }
            else if (settings.charInputStyle == MText_Settings.CharInputStyle.CustomCharacters)
            {
                myChars = settings.customCharacters.ToCharArray().ToList();
            }
            else if (settings.charInputStyle == MText_Settings.CharInputStyle.UnicodeSequence)
            {
                MText_NewFontCharacterRange characterRange = new MText_NewFontCharacterRange();
                myChars = characterRange.RetrieveCharacterListFromUnicodeSequence(settings.unicodeSequence);
            }
            //else if (settings.charInputStyle == MText_Settings.CharInputStyle.CharacterSet)
            //{
            //    MText_NewFontCharacterRange characterRange = new MText_NewFontCharacterRange();
            //    // myChars = characterRange.RetrieveCharacterListFromUnicodeSequence(settings.unicodeSequence);
            //}
            myChars = myChars.Distinct().ToList();

            return myChars;
        }


        #region Layout Item
        private void HorizontalButtonURL(string text, string url)
        {
            if (GUILayout.Button(text, GUILayout.MinHeight(25)))
            {
                Application.OpenURL(url);
            }
        }
        private void GridContent(string title, string description = "", string urlText = "", string url = "")
        {
            Color originalWindowColor = GUI.backgroundColor;
            Color boxBackgroundColor = Color.white;
            if (settings)
                boxBackgroundColor = settings.gridItemColor;
            GUI.backgroundColor = boxBackgroundColor;

            GUILayout.BeginVertical("", gridStyle);
            GUILayout.Space(5);
            GUILayout.Label(title, EditorStyles.boldLabel);
            if (description != string.Empty)
                GUILayout.Label(description, EditorStyles.wordWrappedLabel);

            GUI.backgroundColor = Color.white;
            if (urlText != string.Empty && url != string.Empty)
            {
                if (GUILayout.Button(urlText, GUILayout.MinHeight(25)))
                {
                    Application.OpenURL(url);
                }
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();

            GUI.backgroundColor = originalWindowColor;
        }
        #endregion Layout Item


        #region Style stuff
        void GenerateStyle()
        {
            if (tabStyle == null)
            {
                tabStyle = new GUIStyle(GUI.skin.button);
                tabStyle.margin = new RectOffset(0, 0, tabStyle.margin.top, 0);
                //tabStyle.margin = new RectOffset(0, 0, tabStyle.margin.top, tabStyle.margin.bottom);
                tabStyle.fontStyle = FontStyle.Bold;
            }
            if (gridStyle == null)
            {
                gridStyle = new GUIStyle();
                //gridStyle = new GUIStyle(GUI.skin.box);
                gridStyle.margin = new RectOffset(5, 5, 0, 0);
            }
            if (foldOutStyle == null)
            {
                CreateFoldOutStyle();
            }
        }

        private void CreateFoldOutStyle()
        {
            foldOutStyle = new GUIStyle(EditorStyles.foldout)
            {
                overflow = new RectOffset(-10, 0, 3, 0),
                padding = new RectOffset(15, 0, -3, 0),
                fontStyle = FontStyle.Bold
            };
            foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
        }

        bool LeftButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);
            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(0, 0, rect.width + tabStyle.border.right, rect.height), content, tabStyle))
                clicked = true;

            GUI.EndGroup();
            return clicked;
        }
        bool MidButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 30);
            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-tabStyle.border.left, 0, rect.width + tabStyle.border.left + tabStyle.border.right, rect.height), content, tabStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }
        bool RightButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);
            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-tabStyle.border.left, 0, rect.width + tabStyle.border.left, rect.height), content, tabStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }

        public static void DrawUILine(Color color, int thickness = 1, int padding = 1)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        #endregion
    }
}