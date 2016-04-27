using System;

namespace XBee.Frames
{
    [Flags]
    public enum AnalogSampleChannels : byte
    {
        None = 0x0,
        Input0 = 0x1,
        Input1 = 0x2,
        Input2 = 0x4,
        Input3 = 0x8,
        Input4 = 0x10,
        Input5 = 0x20
    }
}
