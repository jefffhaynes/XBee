using System;

namespace XBee.Frames
{
    [Flags]
    public enum TransmitOptions : byte
    {
        DisableAck = 0x1,
        BroadcastPanId = 0x4,
    }
}
