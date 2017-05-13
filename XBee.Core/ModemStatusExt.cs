namespace XBee
{
    public enum ModemStatusExt : byte
    {
        Rejoin = 0x00,
        StackStatus = 0x01,
        Joining = 0x02,
        Joined = 0x03,
        BeaconResponse = 0x04,
        RejectZigbeeStack = 0x05,
        RejectPanId = 0x06,
        RejectNoJoin = 0x07,
        PanIdMatch = 0x08,
        RejectWeak = 0x09,
        BeaconSaved = 0x0a,
        AssociationChange = 0x0b,
        PermitJoin = 0x0c,
        Scanning = 0x0d,
        ScanError = 0x0e,
        JoinRequest = 0x0f,
        RejectLinkQuality = 0x10,
        RejectRSSI = 0x11,
        RejectCommandLast = 0x12,
        RejectCommandSave = 0x13,
        RejectStrength = 0x14,
        ResetTimeout = 0x16,
        ScanChannel = 0x18,
        ScanMode = 0x19,
        ScanInit = 0x1a,
        ScanEnergyChannelMask = 0x1d,
        ScanEnergyEnergies = 0x1e,
        ScanPanId = 0x1f,
        FormNetwork = 0x20,
        KeyEstablishment = 0x21,
        KeyEstablished = 0x22
    }
}
