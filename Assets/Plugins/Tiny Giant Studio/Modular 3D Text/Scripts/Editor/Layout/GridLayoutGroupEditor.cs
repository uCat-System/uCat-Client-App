using UnityEditor;
using UnityEngine;

namespace MText
{
    [CustomEditor(typeof(GridLayoutGroup))]
    public class GridLayoutGroupEditor : Editor
    {
        GridLayoutGroup myTarget;
        SerializedObject soTarget;

        SerializedProperty autoItemSize;
        SerializedProperty justiceHorizontal;
        SerializedProperty justiceHorizontalPercent;
        SerializedProperty justiceVertical;
        SerializedProperty JusticeVerticalPercent;
        SerializedProperty spacing;
        SerializedProperty width;
        SerializedProperty height;
        SerializedProperty lines;
        SerializedProperty bounds;
        SerializedProperty lineSpacingStyle;

        //style
        private static GUIStyle toggleStyle = null;
        private static GUIStyle foldOutStyle = null;

        bool showDebug = false;
        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);
        private static Color toggledOnButtonColor = Color.white;
        //private static Color toggledOffButtonColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1);
        private static Color toggledOffButtonColor = Color.gray;




        void OnEnable()
        {
            myTarget = (GridLayoutGroup)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            DrawSize();
            DrawAlignment();
            EditorGUILayout.Space(6);
            DrawControls();

            EditorGUILayout.Space(15);
            DrawDebug();

            if (EditorGUI.EndChangeCheck())
            {
                Alignment anchor = myTarget.Anchor;
                if (soTarget.ApplyModifiedProperties())
                {
                    if (myTarget.GetComponent<Modular3DText>())
                    {
                        if (anchor != myTarget.Anchor)
                        {
                            myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                        }
                        else
                        {
                            //if (!myTarget.GetComponent<Modular3DText>().ShouldItCreateChild())
                            {
                                myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(myTarget);
            }
        }

        private void DrawControls()
        {
            MText_Editor_Methods.HorizontalField(spacing, "Spacing");

            if (!myTarget.GetComponent<Modular3DText>())
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(autoItemSize);
                MText_Editor_Methods.HorizontalField(lineSpacingStyle, "Line Spacing Style", "", FieldSize.large);
            }
        }

        private void DrawAlignment()
        {
            Color originalColor = GUI.color;

            GUILayout.BeginHorizontal();

            if (isHorizontallyLeft())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (LeftButton(EditorGUIUtility.IconContent("d_align_horizontally_left")))
            {
                if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.UpperRight)
                    myTarget.Anchor = Alignment.UpperLeft;
                if (myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.MiddleRight)
                    myTarget.Anchor = Alignment.MiddleLeft;
                if (myTarget.Anchor == Alignment.LowerCenter || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.LowerLeft;
            }


            if (isHorizontallyCentered())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (MidButton(EditorGUIUtility.IconContent("d_align_horizontally_center")))
            {
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.UpperRight)
                    myTarget.Anchor = Alignment.UpperCenter;
                if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.MiddleRight)
                    myTarget.Anchor = Alignment.MiddleCenter;
                if (myTarget.Anchor == Alignment.LowerLeft || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.LowerCenter;
            }


            if (isHorizontallyRight())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (RightButton(EditorGUIUtility.IconContent("d_align_horizontally_right")))
            {
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.UpperCenter)
                    myTarget.Anchor = Alignment.UpperRight;
                if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.MiddleCenter)
                    myTarget.Anchor = Alignment.MiddleRight;
                if (myTarget.Anchor == Alignment.LowerLeft || myTarget.Anchor == Alignment.LowerCenter)
                    myTarget.Anchor = Alignment.LowerRight;
            }

            GUI.color = originalColor;

            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

            if (isVerticallyUp())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (LeftButton(EditorGUIUtility.IconContent("d_align_vertically_top")))
            {
                if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.LowerLeft)
                    myTarget.Anchor = Alignment.UpperLeft;
                if (myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.LowerCenter)
                    myTarget.Anchor = Alignment.UpperCenter;
                if (myTarget.Anchor == Alignment.MiddleRight || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.UpperRight;
            }


            if (isVerticallyMiddle())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (MidButton(EditorGUIUtility.IconContent("d_align_vertically_center")))
            {
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.LowerLeft)
                    myTarget.Anchor = Alignment.MiddleLeft;
                if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.LowerCenter)
                    myTarget.Anchor = Alignment.MiddleCenter;
                if (myTarget.Anchor == Alignment.UpperRight || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.MiddleRight;
            }


            if (isVerticallyBottom())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (RightButton(EditorGUIUtility.IconContent("d_align_vertically_bottom")))
            {
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.MiddleLeft)
                    myTarget.Anchor = Alignment.LowerLeft;
                if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.MiddleCenter)
                    myTarget.Anchor = Alignment.LowerCenter;
                if (myTarget.Anchor == Alignment.UpperRight || myTarget.Anchor == Alignment.MiddleRight)
                    myTarget.Anchor = Alignment.LowerRight;
            }


            GUI.color = originalColor;


            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(1.5f), GUILayout.MinWidth(1.5f));

            //var verticalJustice = Resources.Load("Justice Vertical") as Texture;

            //GUIContent verticalJusticeContent;

            //if (verticalJustice)
            //    verticalJusticeContent = new GUIContent(verticalJustice, "Vertical Justice.\nTry to fill the full height with content.");
            //else
            //    verticalJusticeContent = new GUIContent("Justice Horizontal");


            //if (myTarget.JusticeVertical)
            //    GUI.color = toggledOnButtonColor;
            //else
            //    GUI.color = toggledOffButtonColor;

            //if (LeftButton(verticalJusticeContent))
            //{
            //    myTarget.JusticeVertical = !myTarget.JusticeVertical;
            //}

            //if (myTarget.JusticeVertical)
            //{
            //    EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(1), GUILayout.MaxWidth(1));
            //    EditorGUILayout.LabelField(new GUIContent("%", "Justice will be only be applied if the elements hold equal/more than the % height"), GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
            //    EditorGUILayout.PropertyField(JusticeVerticalPercent, GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            //}
            //else
            //{
            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(1), GUILayout.MaxWidth(1));
            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            //}
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            JusticeHorizontal();
            GUILayout.EndHorizontal();

            GUI.color = originalColor;
        }

        private void JusticeHorizontal()
        {
            var justiceHorizontalTexture = Resources.Load("Modular 3D Text/Justice Horizontal") as Texture;

            GUIContent content;

            if (justiceHorizontalTexture)
                content = new GUIContent(justiceHorizontalTexture, "Horizontal Justice.\nTry to fill the full width with content.");
            else
                content = new GUIContent("Justice Horizontal");

            if (myTarget.JusticeHorizontal)
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;

            if (MidButton(content))
            {
                myTarget.JusticeHorizontal = !myTarget.JusticeHorizontal;
            }

            if (myTarget.JusticeHorizontal)
            {
                GUI.color = toggledOnButtonColor;
                EditorGUILayout.LabelField(new GUIContent("%", "Justice will be only be applied if the elements hold equal/more than the % width"), GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
                EditorGUILayout.PropertyField(justiceHorizontalPercent, GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            }
            else
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            }
        }

        bool isHorizontallyLeft()
        {
            if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.LowerLeft)
                return true;
            return false;
        }
        bool isHorizontallyCentered()
        {
            if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.LowerCenter)
                return true;
            return false;
        }
        bool isHorizontallyRight()
        {
            if (myTarget.Anchor == Alignment.UpperRight || myTarget.Anchor == Alignment.MiddleRight || myTarget.Anchor == Alignment.LowerRight)
                return true;
            return false;
        }


        bool isVerticallyUp()
        {
            if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.UpperRight)
                return true;
            return false;
        }
        bool isVerticallyMiddle()
        {
            if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.MiddleRight)
                return true;
            return false;
        }
        bool isVerticallyBottom()
        {
            if (myTarget.Anchor == Alignment.LowerLeft || myTarget.Anchor == Alignment.LowerCenter || myTarget.Anchor == Alignment.LowerRight)
                return true;
            return false;
        }







        private void DrawSize()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.Space(5);

        }


        void DrawDebug()
        {
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDebug = EditorGUILayout.Foldout(showDebug, "Debug", true, foldOutStyle);
            GUILayout.EndVertical();

            if (showDebug)
            {
                //DrawUILine(blueFaded);
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(lines);
                EditorGUILayout.PropertyField(bounds);

                GUILayout.Space(5);
            }
            if (!Selection.activeTransform)
            {
                showDebug = false;
            }
            GUILayout.EndVertical();
        }

        private void FindProperties()
        {
            autoItemSize = soTarget.FindProperty("autoItemSize");

            justiceHorizontal = soTarget.FindProperty("_justiceHorizontal");
            justiceVertical = soTarget.FindProperty("_justiceVertical");
            justiceHorizontalPercent = soTarget.FindProperty("_justiceHorizontalPercent");
            JusticeVerticalPercent = soTarget.FindProperty("_justiceVerticalPercent");
            spacing = soTarget.FindProperty("_spacing");
            width = soTarget.FindProperty("_width");
            height = soTarget.FindProperty("_height");
            lines = soTarget.FindProperty("lines");
            bounds = soTarget.FindProperty("bounds");
            lineSpacingStyle = soTarget.FindProperty("_lineSpacingStyle");
        }

        private void GenerateStyle()
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
    }
}