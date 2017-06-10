using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum PullUpResistorConfiguration : byte
    {
        None = 0x0,
        Ad4Dio4 = 0x01,
        Ad3Dio3 = 0x02,
        Ad2Dio2 = 0x04,
        Ad1Dio1 = 0x08,
        Ad0Dio0 = 0x10,
        Ad6Dio6 = 0x20,
        Di8 = 0x40,
        DinConfig = 0x80
    }
}
