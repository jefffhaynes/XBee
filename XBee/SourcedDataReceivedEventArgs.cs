namespace XBee
{
    public class SourcedDataReceivedEventArgs : DataReceivedEventArgs
    {
        internal SourcedDataReceivedEventArgs(NodeAddress address, byte[] data) : base(data)
        {
            Address = address;
        }

        public NodeAddress Address { get; }
    }
}
