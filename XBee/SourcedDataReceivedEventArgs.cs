namespace XBee
{
    public class SourcedDataReceivedEventArgs : DataReceivedEventArgs
    {
        public SourcedDataReceivedEventArgs(NodeAddress address, byte[] data) : base(data)
        {
            Address = address;
        }

        public NodeAddress Address { get; private set; }
    }
}
