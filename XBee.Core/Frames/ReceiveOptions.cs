using System;

namespace XBee.Frames
{
    [Flags]
    public enum ReceiveOptions : byte
    {
        None = 0x0,
        Broadcast = 0x1,
        PanBroadcast = 0x2
    }
}
