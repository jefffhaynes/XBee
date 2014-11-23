namespace XBee.Frames
{
    public enum DeliveryStatusExt : byte
    {
        Success = 0x00,
        MacAckFailure = 0x01,
        CollisionAvoidanceFailure = 0x02,
        NetworkAckFailure = 0x21,
        RouteNotFound = 0x25,
        InternalResourceError= 0x31,
        InternalError = 0x32,
        PayloadTooLarge = 0x74,
        IndirectMessageUnrequested = 0x75
    }
}
