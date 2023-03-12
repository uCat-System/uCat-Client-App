using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MText
{
    /// <summary>
    /// This class contains different reusable methods for the asset
    /// </summary>
    public class MText_Utilities
    {
        /// <summary>
        /// Returns if an item has a List as parent
        /// </summary>
        /// <param name="transform">The transform being checked</param>
        /// <returns></returns>
        public static MText_UI_List GetParentList(Transform transform)
        {
            if (transform.parent == null)
                return null;

            if (transform.parent.GetComponent<MText_UI_List>())
            {
                return transform.parent.GetComponent<MText_UI_List>();
            }
            else return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Only!
        /// </summary>
        /// <param name="mesh"></param>
        public static void OptimizeMesh(Mesh mesh)
        {
            MeshUtility.Optimize(mesh);
        }
#endif
    }
}