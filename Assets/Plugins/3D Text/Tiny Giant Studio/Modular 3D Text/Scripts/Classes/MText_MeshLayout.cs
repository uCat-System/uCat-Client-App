using UnityEngine;

namespace MText
{
    /// <summary>
    /// This is used by Modular3DText to create combined meshes without instantiating objects
    /// </summary>
    [System.Serializable]
    public class MeshLayout
    {
        public Mesh mesh;
        public Vector3 position;
        public Quaternion rotation = Quaternion.identity;

        public float xOffset = 0;
        public float yOffset = 0;
        public float zOffset = 0;

        public float width = 0;
        public float height = 0;
        public float depth = 0;

        public bool lineBreak;

        public MeshLayout(Mesh newMesh)
        {
            mesh = newMesh;
        }

        public MeshLayout()
        {

        }
    }
}