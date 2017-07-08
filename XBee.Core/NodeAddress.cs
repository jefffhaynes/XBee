using System;

namespace XBee
{
    public class NodeAddress : IEquatable<NodeAddress>
    {
        public static readonly NodeAddress Broadcast = new NodeAddress(LongAddress.Broadcast);

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


        public bool Is16BitDisabled => ShortAddress.Equals(ShortAddress.Broadcast) ||
                                       ShortAddress.Equals(ShortAddress.Disabled);

        public bool Equals(NodeAddress other)
        {
            if (Is16BitDisabled)
            {
                return other != null && LongAddress.Equals(other.LongAddress);
            }

            if (LongAddress.Equals(LongAddress.Broadcast))
            {
                return other != null && ShortAddress.Equals(other.ShortAddress);
            }

            return other != null && LongAddress.Equals(other.LongAddress);
        }

        public override string ToString()
        {
            return $"{LongAddress}, {ShortAddress}";
        }
    }
}