using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.AnimatedValues;
#endif

namespace MText
{
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/scripts/layout-group-2")]
    [AddComponentMenu("Modular 3D Text/Layout/Linear Layout Group (M3D)")]
    public class LinearLayoutGroup : LayoutGroup
    {
        public float spacing = 0;
        public Alignment alignment = Alignment.Left;

        private bool startLoopFromEnd = true;

        public enum Alignment
        {
            Top,
            VerticleMiddle,
            Bottom,
            Left,
            HorizontalMiddle,
            Right
        }


#if UNITY_EDITOR
        /// <summary>
        /// Editor only. 
        /// Don't use it on script.
        /// Used to control the animation of debug group in editor
        /// </summary>
        [HideInInspector] public AnimBool showDebugSettings;
#endif





        [ContextMenu("Update Layout")]
        public override void UpdateLayout()
        {
            if (TotalActiveChildCount() == 0)
                return;

            bounds = GetAllChildBounds();
            float x = 0;
            float y = 0;

            GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            GetLoopStart();

            if (startLoopFromEnd)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i]);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i]);
                }
            }
        }



        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;

            Bounds[] bounds = GetAllChildBounds(meshLayouts);

            float x = 0;
            float y = 0;

            GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            GetLoopStart();

            if (startLoopFromEnd)
            {
                for (int i = meshLayouts.Count - 1; i >= 0; i--)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i]);
                }
            }
            else
            {
                for (int i = 0; i < meshLayouts.Count; i++)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i]);
                }
            }

            return meshLayouts;
        }

        private MeshLayout SetMeshPosition(ref float x, ref float y, Bounds bound, MeshLayout meshLayout)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;


                x -= bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            meshLayout.position = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            //transform.GetChild(i).localPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            return meshLayout;
        }


        private void SetChildPosition(ref float x, ref float y, int i, Bounds bound)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;


                x -= bound.center.x;
            }
            x += toAddX;
            y += toAddY;

#if UNITY_EDITOR
            ///This is to avoid layout group setting the objects dirty for no reason
            bool setDirty = false;
            if (!Application.isPlaying)
            {
                Vector3 oldPos = transform.GetChild(i).localPosition;
                if (oldPos != RemoveNaNErrorIfAny(new Vector3(x, y, 0)))
                    setDirty = true;
            }
#endif
            transform.GetChild(i).localPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

#if UNITY_EDITOR
            if (setDirty)
                UnityEditor.EditorUtility.SetDirty(transform.GetChild(i));
#endif

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }
            x += toAddX;
            y += toAddY;
        }



        private void GetPositionValuesAccordingToSelectedLayout(ref float x, ref float y, Bounds[] bounds)
        {
            if (alignment == Alignment.Bottom)
            {
                y = spacing / 2;
            }
            else if (alignment == Alignment.VerticleMiddle)
            {
                for (int i = 0; i < bounds.Length; i++)
                {
                    if (bounds[i].size == Vector3.zero)
                        continue;

                    y += bounds[i].size.y + spacing;
                }

                y /= 2;
            }
            else if (alignment == Alignment.Top)
            {
                y = -spacing / 2;
            }


            else if (alignment == Alignment.Left)
            {
                x = -spacing / 2;
            }
            else if (alignment == Alignment.HorizontalMiddle)
            {
                for (int i = 0; i < bounds.Length; i++)
                {
                    if (bounds[i].size == Vector3.zero)
                        continue;

                    x += bounds[i].size.x + spacing;
                }

                x /= 2;
            }
            else if (alignment == Alignment.Right)
            {
                x = (spacing / 2);
            }
        }

        void GetLoopStart()
        {
            if (alignment == Alignment.Top)
            {
                startLoopFromEnd = true;
            }
            else if (alignment == Alignment.Bottom)
            {
                startLoopFromEnd = false;
            }
            else if (alignment == Alignment.VerticleMiddle)
            {
                startLoopFromEnd = false;
            }

            else if (alignment == Alignment.Left)
            {
                startLoopFromEnd = false;
            }
            else if (alignment == Alignment.HorizontalMiddle)
            {
                startLoopFromEnd = true;
            }
            else if (alignment == Alignment.Right)
            {
                startLoopFromEnd = true;
            }
        }
    }
}