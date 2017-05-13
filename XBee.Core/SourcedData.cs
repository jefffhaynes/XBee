namespace XBee
{
    public class SourcedData : Sourced
    {
        internal SourcedData(NodeAddress address, byte[] data) : base(address)
        {
            Data = data;
        }
        
        public byte[] Data { get; }
    }
}
