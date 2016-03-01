using System;

namespace XBee
{
    public class NodeAddress : IEquatable<NodeAddress>
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

        public NodeAddress(ShortAddress shortAddress) : this(LongAddress.Broadcast, shortAddress)
        {
        }

        public LongAddress LongAddress { get; set; }

        public ShortAddress ShortAddress { get; set; }


        public bool Is16BitDisabled => ShortAddress.Equals(ShortAddress.Broadcast) || ShortAddress.Equals(ShortAddress.Disabled);

        public bool Equals(NodeAddress other)
        {
            if (Is16BitDisabled)
                return LongAddress.Equals(other.LongAddress);

            if (LongAddress.Equals(LongAddress.Broadcast))
                return ShortAddress.Equals(other.ShortAddress);

            return LongAddress.Equals(other.LongAddress);
        }

        public override string ToString()
        {
            return $"{LongAddress}, {ShortAddress}";
        }
    }
}
