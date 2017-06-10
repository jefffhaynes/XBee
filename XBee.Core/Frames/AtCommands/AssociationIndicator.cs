namespace XBee.Frames.AtCommands
{
    public enum AssociationIndicator : byte
    {
        Success = 0x0,
        ScanTimeout = 0x1,
        ActiveScanNoPan = 0x2,
        NoAssociationAllowed = 0x3,
        NoBeaconSupport = 0x4,
        IdDoesNotMatch = 0x5,
        ChannelDoesNotMatch = 0x6,
        EnergyScanTimeout = 0x7,
        CoordinatorStartRequestFailed = 0x8,
        CoordinatorFailedInvalidParameter = 0x9,
        CoordinatorRealigning = 0xa,
        AssociationRequestNotSent = 0xb,
        AssociationRequestFailedTimeout = 0xc,
        AssociationRequestFailedInvalidParameter = 0xd,
        AssociationRequestChannelAccessFailure = 0xe,
        AssociationRequestNoAck = 0xf,
        AssociationRequestNoReply = 0x10,
        CoordinatorSynchronizationLost = 0x12,
        Disassociated = 0x13,
        ScanNoPan = 0x21,
        ScanNoValidPan = 0x22,
        NoJoinAllowed = 0x23,
        NoJoinable = 0x24,
        InvalidState = 0x25,
        JoinFailed = 0x27,
        CoordinatorStartFailed = 0x2a,
        SearchingForCoordinator = 0x2b,
        NetworkLeaveFailed = 0x2c,
        JoinUnresponsive = 0xab,
        JoinErrorUnsecuredKey = 0xac,
        JoinErrorNoKey = 0xad,
        JoinErrorWrongKey= 0xaf,
        Scanning = 0xff
    }
}
