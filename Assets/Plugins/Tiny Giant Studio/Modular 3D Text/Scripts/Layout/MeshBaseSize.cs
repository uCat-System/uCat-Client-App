using UnityEngine;

namespace MText
{
    public class MeshBaseSize : MonoBehaviour
    {
        //Commented out because it's unused
        //public static Bounds CheckMeshSize(Transform target, Transform relativeTo)
        //{
        //    Bounds bound = CheckMeshSize(target);
        //    bound.center = -target.position;
        //    return bound;
        //}

        public static Bounds CheckMeshSize(Transform target)
        {
            var currentRotation = target.localRotation;
            target.localRotation = Quaternion.Euler(Vector3.zero);

            Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);

            var meshFilters = target.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; ++i)
            {
                Transform filterTransform = meshFilters[i].transform;

                if (!meshFilters[i].sharedMesh)
                    continue;

                Bounds bounds = meshFilters[i].sharedMesh.bounds;

                Vector3 scaleFixer = Divide(filterTransform.lossyScale, target.lossyScale);

                bounds.size = Multiply(bounds.size, scaleFixer);
                //bounds.center = Multiply(bounds.center, scaleFixer);

                if (i == 0)
                {
                    newBounds = bounds;
                }
                else
                {
                    bounds.center += filterTransform.localPosition;
                    newBounds.Encapsulate(bounds);
                }
            }


            newBounds.size = Multiply(newBounds.size, target.lossyScale);
            newBounds.center = Multiply(newBounds.center, target.lossyScale);
            //newBounds.size = new Vector3((target.localScale.x / target.lossyScale.x) * newBounds.size.x, (target.localScale.y / target.lossyScale.y) * newBounds.size.y, (target.localScale.z / target.lossyScale.z) * newBounds.size.z);

            target.localRotation = currentRotation;
            //Debug.Log(newBounds);
            return newBounds;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Bounds CheckMeshScaledSize(Transform target)
        {
            var currentRotation = target.localRotation;
            target.localRotation = Quaternion.Euler(Vector3.zero);

            Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);

            var meshFilters = target.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; ++i)
            {
                Transform filterTransform = meshFilters[i].transform;

                if (!meshFilters[i].sharedMesh)
                    continue;

                Bounds bounds = meshFilters[i].sharedMesh.bounds;

                Vector3 scaleFixer = Divide(filterTransform.lossyScale, target.lossyScale);

                bounds.size = Multiply(bounds.size, scaleFixer);
                //bounds.center = Multiply(bounds.center, scaleFixer);

                if (i == 0)
                {
                    newBounds = bounds;
                }
                else
                {
                    bounds.center += filterTransform.localPosition;
                    newBounds.Encapsulate(bounds);
                }
            }


            newBounds.size = Multiply(newBounds.size, target.localScale);
            newBounds.center = Multiply(newBounds.center, target.localScale);
            //newBounds.size = new Vector3((target.localScale.x / target.lossyScale.x) * newBounds.size.x, (target.localScale.y / target.lossyScale.y) * newBounds.size.y, (target.localScale.z / target.lossyScale.z) * newBounds.size.z);

            target.localRotation = currentRotation;
            //Debug.Log(newBounds);
            return newBounds;
        }


        public static Bounds CheckMeshSize(Mesh mesh)
        {
            if (!mesh)
                return new Bounds(Vector3.zero, Vector3.zero);

            return mesh.bounds;
        }




        private static Vector3 Divide(Vector3 first, Vector3 second)
        {
            return new Vector3(NanFixed(first.x / second.x), NanFixed(first.y / second.y), NanFixed(first.z / second.z));
        }
        private static Vector3 Multiply(Vector3 first, Vector3 second)
        {
            return new Vector3(NanFixed(first.x * second.x), NanFixed(first.y * second.y), NanFixed(first.z * second.z));
        }

        static float NanFixed(float value)
        {
            if (float.IsNaN(value))
            {
                return 1;
            }
            return value;
        }
    }
}