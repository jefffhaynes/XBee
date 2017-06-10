using System;

namespace XBee.Frames
{
    [Flags]
    public enum DigitalSampleState : ushort
    {
        None = 0x0,
        Input0 = 0x1,
        Input1 = 0x2,
        Input2 = 0x4,
        Input3 = 0x8,
        Input4 = 0x10,
        Input5 = 0x20,
        Input6 = 0x40,
        Input7 = 0x80,
        Input8 = 0x100,
        Input9 = 0x200,
        Input10 = 0x400,
        Input11 = 0x800,
        Input12 = 0x1000,
        Input13 = 0x2000,
        Input14 = 0x4000,

        All = Input0 | Input1 | Input2 | Input3 | Input4 | Input5
                | Input6 | Input7 | Input8 | Input9 | Input10 | Input11 | Input12 | Input13 | Input14
    }
}
