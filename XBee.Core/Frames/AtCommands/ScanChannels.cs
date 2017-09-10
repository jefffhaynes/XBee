using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum ScanChannels : ushort
    {
        Channel0B = 0b0000_0000_0000_0001,
        Channel0C = 0b0000_0000_0000_0010,
        Channel0D = 0b0000_0000_0000_0100,
        Channel0E = 0b0000_0000_0000_1000,
        Channel0F = 0b0000_0000_0001_0000,
        Channel10 = 0b0000_0000_0010_0000,
        Channel11 = 0b0000_0000_0100_0000,
        Channel12 = 0b0000_0000_1000_0000,
        Channel13 = 0b0000_0001_0000_0000,
        Channel14 = 0b0000_0010_0000_0000,
        Channel15 = 0b0000_0100_0000_0000,
        Channel16 = 0b0000_1000_0000_0000,
        Channel17 = 0b0001_0000_0000_0000,
        Channel18 = 0b0010_0000_0000_0000,
        Channel19 = 0b0100_0000_0000_0000,
        Channel1A = 0b1000_0000_0000_0000,
    }
}
