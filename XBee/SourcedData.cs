namespace XBee
{
    public class SourcedData
    {
        internal SourcedData(NodeAddress address, byte[] data)
        {
            Address = address;
            Data = data;
        }

        public NodeAddress Address { get; private set; }

        public byte[] Data { get; private set; }
    }
}
