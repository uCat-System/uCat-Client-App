using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/~/drafts/-Mg0kGcEvjXVrKeBZ0No/scripts/layout-group")]

    public abstract class LayoutGroup : MonoBehaviour
    {
        [Tooltip("This is an experipental feature. In text, this bool is ignored, turn it on from the text itself. \nIf the child element has a Layout Element component attached, this value is derived by that layout element component.")]
        public bool autoItemSize = false;
        [Tooltip("Auto updated with the layout. Visible for debugging purposes.")]
        public Bounds[] bounds;


#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void Update()
        {
            if (Application.isPlaying)
                this.enabled = false;
            else if (TotalActiveChildCount() > 0)
            {
                if (GetComponent<Modular3DText>())
                    if (GetComponent<Modular3DText>().combineMeshInEditor)
                        return;

                UpdateLayout();
            }
        }
#endif

        public abstract void UpdateLayout();

        /// <summary>
        /// Used to retrieve appropriate positions of meshes without needing to place them on scene for single mesh
        /// </summary>
        public abstract List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts);

        public int TotalActiveChildCount()
        {
            int child = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                    child++;
            }

            return child;
        }

        public Bounds GetBound(Transform target)
        {
            LayoutElement element = target.GetComponent<LayoutElement>();

            if (element)
                if (element.autoCalculateSize)
                    return MeshBaseSize.CheckMeshScaledSize(target);
                else
                    return new Bounds(new Vector3(element.xOffset, element.yOffset, element.zOffset), new Vector3(element.width, element.height, element.depth));


            if (autoItemSize)
                return MeshBaseSize.CheckMeshScaledSize(target);
            else
                return new Bounds(Vector3.zero, Vector3.one);
        }

        public Bounds GetBound(MeshLayout meshLayout)
        {
            return new Bounds(new Vector3(meshLayout.xOffset, meshLayout.yOffset, meshLayout.zOffset), new Vector3(meshLayout.width, meshLayout.height, meshLayout.depth));
        }


        public Bounds[] GetAllChildBounds()
        {
            Bounds[] bounds = new Bounds[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                bounds[i] = GetBound(transform.GetChild(i));
            }
            return bounds;
        }

        public Bounds[] GetAllChildBounds(List<MeshLayout> meshLayouts)
        {
            Bounds[] bounds = new Bounds[meshLayouts.Count];
            for (int i = 0; i < meshLayouts.Count; i++)
            {
                bounds[i] = GetBound(meshLayouts[i]);
            }
            return bounds;
        }

        public bool IgnoreChildBound(Bounds[] bounds, int i)
        {
            if (transform.GetChild(i).GetComponent<LayoutElement>())
                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement == true)
                    return true;

            return !transform.GetChild(i).gameObject.activeSelf || bounds[i].size == Vector3.zero;
        }
        public bool IgnoreChildBoundAndLineBreak(Bounds[] bounds, int i)
        {
            if (transform.GetChild(i).GetComponent<LayoutElement>())
                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement == true || transform.GetChild(i).GetComponent<LayoutElement>().lineBreak == true)
                    return true;

            return !transform.GetChild(i).gameObject.activeSelf || bounds[i].size == Vector3.zero;
        }

        public Vector3 RemoveNaNErrorIfAny(Vector3 vector3)
        {
            if (float.IsNaN(vector3.x))
                vector3.x = 0;
            if (float.IsNaN(vector3.y))
                vector3.y = 0;
            if (float.IsNaN(vector3.z))
                vector3.z = 0;

            return vector3;
        }



        /// <summary>
        /// Draws each element bound
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.25f, 0, 0, 1f);
            Vector3 lossyScale = transform.lossyScale;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i))
                    continue;

                Bounds bound = GetBound(transform.GetChild(i));
                Gizmos.matrix = Matrix4x4.TRS(transform.GetChild(i).position, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Multiply(bound.center, lossyScale), Multiply(bound.size, lossyScale));
                //Gizmos.DrawWireCube(Multiply(bound.center, lossyScale) + transform.GetChild(i).position, Multiply(bound.size, lossyScale));
            }
        }
        private Vector3 Multiply(Vector3 first, Vector3 second)
        {
            return new Vector3(first.x * second.x, first.y * second.y, first.z * second.z);
        }
    }
}