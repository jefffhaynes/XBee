using System;
using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    [Flags]
    public enum ReceiveOptions : byte
    {
        None = 0x0,
        Broadcast = 0x1,
        PanBroadcast = 0x2
    }
}
