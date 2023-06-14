using UnityEngine;

namespace MText
{
    public class LayoutElement : MonoBehaviour
    {
        [Tooltip("Ignores this element in layout group")]
        public bool ignoreElement;

        [Tooltip("This is overwritten on texts")]
        public bool autoCalculateSize = false;
        //TODO: Custom editor to make these read only when autocalculate size is open
        public float width = 1;
        public float height = 1;
        [HideInInspector]
        public float depth = 1; //TODO


        public float xOffset = 0;
        public float yOffset = 0;
        public float zOffset = 0;

        [Tooltip("Used in Grid layout")]
        public bool lineBreak = false;
    }
}