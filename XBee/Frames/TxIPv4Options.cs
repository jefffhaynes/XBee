using System;

namespace XBee.Frames
{
    [Flags]
    public enum TxIPv4Options : byte
    {
        None = 0x0,
        CloseSocketAfterSuccess = 0x1
    }
}
