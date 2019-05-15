using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public abstract class Sourced
    {
        protected Sourced(NodeAddress address)
        {
            Address = address;
        }

        public NodeAddress Address { get; }
    }
}
