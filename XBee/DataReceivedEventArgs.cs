using System;

namespace XBee
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(LongAddress source, byte[] data)
        {
            Source = source;
            Data = data;
        }

        public LongAddress Source { get; private set; }

        public byte[] Data { get; private set; }
    }
}
