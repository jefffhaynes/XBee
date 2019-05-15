using System;
using JetBrains.Annotations;

namespace XBee.Frames
{
    [PublicAPI]
    [Flags]
    public enum TxIPv4Options : byte
    {
        None = 0x0,
        CloseSocketAfterSuccess = 0x1
    }
}
