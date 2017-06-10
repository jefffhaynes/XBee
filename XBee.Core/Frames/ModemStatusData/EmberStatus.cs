namespace XBee.Frames.ModemStatusData
{
    public enum EmberStatus : byte
    {
        NoNetwork = 0x00,
        Joining = 0x01,
        Joined = 0x02,
        JoinedNoParent = 0x03,
        Leaving = 0x04
    }
}
