namespace XBee
{
    public enum ModemStatus : byte
    {
        HardwareReset = 0x0,
        WatchdogReset = 0x1,
        Associated = 0x2,
        Disassociated = 0x3,
        SynchronizationLost = 0x4,
        CoordinatorRealignment = 0x5,
        CoordinatorStarted = 0x6,
        NetworkSecurityKeyUpdated = 0x7,
        NetworkWake = 0xB,
        NetworkSleep = 0xC,
        VoltageSupplyLimitExceeded = 0xD,
        ModemConfigChangedDuringJoin = 0x11
    }
}
