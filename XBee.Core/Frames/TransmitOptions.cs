using System;
using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    [Flags]
    public enum TransmitOptions : byte
    {
        DisableAck = 0x1,
        BroadcastPanId = 0x4,
    }
}
