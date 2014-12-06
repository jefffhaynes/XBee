using System;

namespace XBee
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(NodeAddress address, byte[] data)
        {
            Address = address;
            Data = data;
        }

        public NodeAddress Address { get; private set; }

        public byte[] Data { get; private set; }
    }
}
