namespace XBee.Frames
{
    public enum DeliveryStatusExt : byte
    {
        Success = 0x00,
        MacAckFailure = 0x01,
        CollisionAvoidanceFailure = 0x02,
        NetworkAckFailure = 0x21,
        NotJoinedToNetwork = 0x22,
        SelfAddressed = 0x23,
        AddressNotFound = 0x24,
        RouteNotFound = 0x25,
        NeighborFailure = 0x26,
        InvalidBindingTableIndex = 0x2b,
        ResourceError = 0x2c,
        AttemptedBroadcast = 0x2d,
        AttemptedUnicast = 0x2e,
        InternalResourceError= 0x31,
        InternalError = 0x32,
        PayloadTooLarge = 0x74,
        IndirectMessageUnrequested = 0x75
    }
}
