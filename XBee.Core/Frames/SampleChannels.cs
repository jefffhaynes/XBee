using System;

namespace XBee.Frames
{
    [Flags]
    public enum SampleChannels : ushort
    {
        None = 0x0,
        Digital0 = 0x1,
        Digital1 = 0x2,
        Digital2 = 0x4,
        Digital3 = 0x8,
        Digital4 = 0x10,
        Digital5 = 0x20,
        Digital6 = 0x40,
        Digital7 = 0x80,
        Digital8 = 0x100,
        AllDigital = Digital0 | Digital1 | Digital2 | Digital3 | Digital4 | Digital5 | Digital6 | Digital7 | Digital8,
        Analog0 = 0x200,
        Analog1 = 0x400,
        Analog2 = 0x800,
        Analog3 = 0x1000,
        Analog4 = 0x2000,
        Analog5 = 0x4000,
        AllAnalog = Analog0 | Analog1 | Analog2 | Analog3 | Analog4 | Analog5
    }
}
