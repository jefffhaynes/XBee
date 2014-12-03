using System;

namespace XBee.Frames.AtCommands
{
    [Flags]
    public enum SleepOptionsExt : byte
    {
        None = 0x0,
        PreferredSleepCoordinator = 0x1,
        NonSleepCoordinator = 0x2,
        SleepStatusMessageEnable = 0x3,
        EarlyWakeDisable = 0x4,
        NodeTypeEqualityEnable = 0x5,
        LoneCoordinatorSyncRepeatDisable = 0x6
    }
}
