using System;

namespace XBee
{
    public class NodeAddress
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

        public NodeAddress(LongAddress longAddress) : this(longAddress, ShortAddress.Disabled)
        {
        }

        public NodeAddress(ShortAddress shortAddress) : this(LongAddress.Disabled, shortAddress)
        {
        }

        public LongAddress LongAddress { get; }

        public ShortAddress ShortAddress { get; }

        public bool Equals(NodeAddress other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(LongAddress, other.LongAddress) && Equals(ShortAddress, other.ShortAddress);
        }


        public override string ToString()
        {
            return $"{LongAddress}, {ShortAddress}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((NodeAddress) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((LongAddress != null ? LongAddress.GetHashCode() : 0) * 397) ^
                       (ShortAddress != null ? ShortAddress.GetHashCode() : 0);
            }
        }
    }
}