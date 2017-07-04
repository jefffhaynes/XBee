using System;

namespace XBee
{
    public class DataReceivedEventArgs : EventArgs
    {
        internal DataReceivedEventArgs(NodeAddress address, byte[] data)
        {
            Address = address;
            Data = data;
        }

        public NodeAddress Address { get; }

        public byte[] Data { get; }
    }
}
