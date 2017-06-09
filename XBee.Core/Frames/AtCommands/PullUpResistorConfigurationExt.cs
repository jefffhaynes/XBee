using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum PullUpResistorConfigurationExt : ushort
    {
        None = 0x0000,
        Dio4 = 0x0001,
        Ad3Dio3 = 0x0002,
        Ad2Dio2 = 0x0004,
        Ad1Dio1 = 0x0008,
        Ad0Dio0 = 0x0010,
        RtsDio6 = 0x0020,
        DtrSleepDio8 = 0x0040,
        DinConfig = 0x0080,
        Associate = 0x0100,
        OnSleep = 0x0200,
        Dio12 = 0x0400,
        Pwm0RssiDio10 = 0x0800,
        Pwm1Dio11 = 0x1000,
        CtsDio7 = 0x2000
    }
}
