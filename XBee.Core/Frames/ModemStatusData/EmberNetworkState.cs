namespace XBee.Frames.ModemStatusData
{
    public enum EmberNetworkState : byte
    {
        NetworkUp = 0x90,
        NetworkDown = 0x91,
        JoinFailed = 0x94,
        ContactFailed = 0x96,
        RouterJoinFailed = 0x98,
        NetworkIdChanged = 0x99,
        PanIdChanged = 0x9a,
        ChannelChanged = 0x9b,
        NoBeacons = 0xab
    }
}
