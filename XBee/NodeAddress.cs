namespace XBee
{
    public class NodeAddress
    {
        public NodeAddress()
        {
        }

        public NodeAddress(LongAddress longAddress, ShortAddress shortAddress)
        {
            LongAddress = longAddress;
            ShortAddress = shortAddress;
        }

        public NodeAddress(LongAddress longAddress) : this(longAddress, ShortAddress.Broadcast)
        {
        }

        public NodeAddress(ShortAddress shortAddress) : this(LongAddress.BroadcastAddress, shortAddress)
        {
        }

        public LongAddress LongAddress { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}", LongAddress, ShortAddress);
        }
    }
}
