namespace XBee
{
    public class SourcedDataReceivedEventArgs : DataReceivedEventArgs
    {
        public SourcedDataReceivedEventArgs(NodeAddress sourceAddress, byte[] data) : base(sourceAddress, data)
        {
        }
    }
}
