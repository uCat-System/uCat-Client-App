namespace MText
{
    [System.Serializable]
    public struct MText_KernPairHolder
    {
        public MText_KernPair kernPair;
        public int offset;

        public MText_KernPairHolder(MText_KernPair kernPair, short offset)
        {
            this.kernPair = kernPair;
            this.offset = offset;
        }
    }
}