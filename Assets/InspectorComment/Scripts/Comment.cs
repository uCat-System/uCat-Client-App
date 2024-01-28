namespace K5F.InspectorComment
{
    using UnityEngine;
    using System;
#if UNITY_EDITOR
    using static GizmoIcons;
#endif

    /// <summary>
    /// Shows editable comments in the Unity Inspector.
    /// Use the context menu "Edit Comment" to edit the comment.
    /// </summary>
    public class Comment : MonoBehaviour
    {
        [Serializable]
        public enum EKind
        {
            Info, Warning, ToDo
        }

        [SerializeField] internal EKind kind = EKind.Info;
        [SerializeField] internal string message = "Lorem ipsum dolor sit amet.";

        [SerializeField, HideInInspector] internal bool isEditing = true;

#if UNITY_EDITOR
        [ContextMenu("Edit Comment")]
        private void SetEditing()
        {
            isEditing = true;
        }
        
        private void OnDrawGizmos()
        {
            switch (kind)
            {
                case EKind.Warning:
                case EKind.ToDo:
                    Gizmos.DrawIcon(transform.position, GetFilePath(kind), true);
                    break;
                default:
                    break;
            }
        }
#endif
    }
}
