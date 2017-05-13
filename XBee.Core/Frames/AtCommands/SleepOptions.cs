using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum SleepOptions : byte
    {
        None = 0x0,
        PollWakeupDisable = 0x1,
        SampleOnWakeDisable = 0x2
    }
}
