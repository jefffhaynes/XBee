namespace XBee.Frames.AtCommands
{
    public enum SleepMode : byte
    {
        Disabled = 0x0,
        PinSleep = 0x1,
        PinDoze = 0x2,
        CyclicSleep = 0x4,
        CyclicSleepWithPinWake = 0x5,
        SleepCoordinator = 0x6,
        SynchronousSleepSupport = 0x7,
        SynchronousCyclicSleep = 0x8
    }
}
