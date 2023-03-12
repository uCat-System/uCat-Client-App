using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MText
{
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/scripts/layout-group-3")]
    [AddComponentMenu("Modular 3D Text/Layout/Circular Layout Group (M3D)")]
    public class CircularLayoutGroup : LayoutGroup
    {
        public Direction direction;
        public enum Direction
        {
            left,
            right
        }

        public Style style;
        public enum Style
        {
            style1,
            style2,
            style3,
            style4,
            style5
        }

        public bool useRotation = false;
        public Vector3 rotation;
        public float spread = 360;
        public float radius = 5;
        public float radiusDecreaseRate = 0;

        //private float angle;
        private int totalActiveChildCount;
        private float xPos;
        private float yPos;






        [ContextMenu("Update Layout")]
        public override void UpdateLayout()
        {
            totalActiveChildCount = TotalActiveChildCount();
            if (totalActiveChildCount == 0)
                return;

            bounds = GetAllChildBounds();
            float totalSize = TotalXSize(bounds);
            //float totalSize = TotalYSize(bounds);

            float angle = 0;
            float currentRadius = radius;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    float toAdd = (spread) * (Size(bounds[i], totalSize)) / 2;

                    if (direction == Direction.left)
                        angle += toAdd;
                    else
                        angle -= toAdd;


                    xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * currentRadius;
                    yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * currentRadius;

                    transform.GetChild(i).localPosition = new Vector3(xPos, yPos, 0);

                    transform.GetChild(i).localRotation = GetRotation(angle, i);

                    if (direction == Direction.left)
                        angle += toAdd;
                    else
                        angle -= toAdd;

                    currentRadius -= radiusDecreaseRate;
                }
            }
        }


        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;
            bounds = GetAllChildBounds(meshLayouts);
            float totalSize = TotalXSize(bounds);
            //float totalSize = TotalYSize(bounds);

            float angle = 0;
            float currentRadius = radius;

            for (int i = 0; i < meshLayouts.Count; i++)
            {
                float toAdd = (spread) * (Size(bounds[i], totalSize)) / 2;

                if (direction == Direction.left)
                    angle += toAdd;
                else
                    angle -= toAdd;


                xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * currentRadius;
                yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * currentRadius;

                meshLayouts[i].position = new Vector3(xPos, yPos, 0);
                meshLayouts[i].rotation = GetRotation(angle, i);

                if (direction == Direction.left)
                    angle += toAdd;
                else
                    angle -= toAdd;

                currentRadius -= radiusDecreaseRate;
            }

            return meshLayouts;
        }

        private float Size(Bounds myBound, float totalBound)
        {
            return myBound.size.x / totalBound;
            //return myBound.size.y / totalBound;
        }

        float TotalXSize(Bounds[] bounds)
        {
            float x = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                x += bounds[i].size.y;
            }

            return x;
        }
        float TotalYSize(Bounds[] bounds)
        {
            float y = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                y += bounds[i].size.y;
            }

            return y;
        }

        private Quaternion GetRotation(float angle, int i)
        {
            if (!useRotation)
                return GetRotationFromStyle(angle, i);
            else
                return GetRotationFromFlatRotation(angle);
        }
        private Quaternion GetRotationFromFlatRotation(float angle)
        {
            return Quaternion.Euler(angle - rotation.x, rotation.y, rotation.z);
        }
        private Quaternion GetRotationFromStyle(float angle, int i)
        {
            //centered
            if (style == Style.style1)
                return Quaternion.Euler(angle - 90, 90, 0);
            else if (style == Style.style2)
                return Quaternion.Euler(angle + 90, 90, 0);
            else if (style == Style.style3)
                return Quaternion.Euler(angle + 90, 90, 90);
            else if (style == Style.style4)
                return Quaternion.Euler(angle - 90, 90, 90);
            else
            {
                Vector3 toTargetVector;
                if (transform.childCount > i)
                    toTargetVector = Vector3.zero - transform.GetChild(i).localPosition;
                else
                    toTargetVector = Vector3.zero;

                float zRotation = Mathf.Atan2(toTargetVector.y, toTargetVector.x) * Mathf.Rad2Deg;
                return Quaternion.Euler(new Vector3(0, 0, zRotation));
            }
        }



#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0.75f, 0.75f, 1f, 0.9f);

            float gizmoAngle = 0;

            int totalItems = Mathf.RoundToInt(radius * 4);
            if (totalItems < 10)
                totalItems = 10;

            if (direction == Direction.left)
                gizmoAngle = 0;
            else
                gizmoAngle = -(spread);
            //gizmoAngle = -(spread / 2);

            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i <= totalItems; i++)
            {
                float xPos = Mathf.Sin(Mathf.Deg2Rad * gizmoAngle) * radius;
                float yPos = Mathf.Cos(Mathf.Deg2Rad * gizmoAngle) * radius;

                Vector3 newPos = new Vector3(xPos, yPos, 0);
                points.Add(newPos);

                gizmoAngle += (spread / totalItems);

            }
            Handles.DrawAAPolyLine(5, points.ToArray());
        }
#endif
    }
}