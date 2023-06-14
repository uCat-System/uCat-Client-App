using UnityEngine;

namespace MText
{
    [System.Serializable]
    public class MText_Character
    {
        public char character;
        public GameObject prefab;
        public Mesh meshPrefab;

        public int glyphIndex;
        /// <summary>
        /// Named advance in typeface. Can't rename here because it would break a lot of existing fonts
        /// </summary>
        public float spacing = 700; 

        public float xOffset;
        public float yOffset;
        public float zOffset;
    }
}
