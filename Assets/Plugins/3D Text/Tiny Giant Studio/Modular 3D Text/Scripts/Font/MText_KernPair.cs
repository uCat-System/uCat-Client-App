namespace MText
{
    [System.Serializable]
    public struct MText_KernPair
    {
        public int left;
        public int right;

        public MText_KernPair(int left, int right)
        {
            this.left = left;
            this.right = right;
        }
    }
}
