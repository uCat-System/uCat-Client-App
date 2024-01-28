namespace K5F.InspectorComment.Editor
{
    using static Comment;
    using static GizmoIcons;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Custom inspector for <see cref="Comment"/> script.
    /// Shows the icon for the kind of comment and static/editable text.
    /// </summary>
    [CustomEditor(typeof(Comment))]
    public class Comment_Inspector : Editor
    {
        private SerializedProperty ser_kind;
        private EKind Kind
        {
            get => (EKind)ser_kind.enumValueIndex;
            set => ser_kind.enumValueIndex = (int)value;
        }

        private SerializedProperty ser_message;
        private string Message
        {
            get => ser_message.stringValue;
            set => ser_message.stringValue = value;
        }

        private SerializedProperty ser_isEditing;
        private bool IsEditing
        {
            get => ser_isEditing.boolValue;
            set => ser_isEditing.boolValue = value;
        }

        private Texture infoTex, warningTex, todoTex;
        private GUIStyle texGuiStyle;
        private GUIStyle labelGuiStyle;

        private void OnEnable()
        {
            ser_kind = serializedObject.FindProperty("kind");
            ser_message = serializedObject.FindProperty("message");
            ser_isEditing = serializedObject.FindProperty("isEditing");

            infoTex = Resources.Load<Texture>(GetIconNameForKind(EKind.Info));
            warningTex = Resources.Load<Texture>(GetIconNameForKind(EKind.Warning));
            todoTex = Resources.Load<Texture>(GetIconNameForKind(EKind.ToDo));
            texGuiStyle = new GUIStyle();
            texGuiStyle.fixedHeight = 64;
            texGuiStyle.fixedWidth = 64;
            labelGuiStyle = new GUIStyle();
            labelGuiStyle.clipping = TextClipping.Clip;
            labelGuiStyle.wordWrap = true;
            labelGuiStyle.normal.textColor = Color.white;
        }

        private Texture TexForKind()
        {
            return Kind switch
            {
                EKind.Info => infoTex,
                EKind.Warning => warningTex,
                EKind.ToDo => todoTex,
                _ => infoTex
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            bool toggle = false;

            if (IsEditing)
            {
                GUILayout.BeginHorizontal();
                var tex = TexForKind();
                if (tex != null)
                {
                    GUILayout.Box(tex, style: texGuiStyle);
                }
                else
                {
                    EditorGUILayout.LabelField(Kind.ToString());
                }
                EditorGUILayout.PropertyField(ser_kind);
                GUILayout.EndHorizontal();
                Message = GUILayout.TextArea(Message);

                toggle = GUILayout.Button("Done");
            }
            else
            {
                GUILayout.BeginHorizontal();
                var tex = TexForKind();
                if (tex != null)
                {
                    GUILayout.Box(tex, style: texGuiStyle);
                }
                else
                {
                    EditorGUILayout.LabelField(Kind.ToString());
                }
                GUILayout.Label(Message, labelGuiStyle);
                GUILayout.EndHorizontal();
            }

            if (toggle)
                IsEditing = !IsEditing;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
