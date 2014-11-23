namespace XBee
{
    public enum ModemStatus : byte
    {
        HardwareReset = 0x0,
        WatchdogReset = 0x1,
        NetworkWake = 0xB,
        NetworkSleep = 0xC
    }
}
