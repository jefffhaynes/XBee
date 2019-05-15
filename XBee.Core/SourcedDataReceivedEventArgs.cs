using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public class SourcedDataReceivedEventArgs : DataReceivedEventArgs
    {
        public SourcedDataReceivedEventArgs(NodeAddress sourceAddress, byte[] data) : base(sourceAddress, data)
        {
        }
    }
}
