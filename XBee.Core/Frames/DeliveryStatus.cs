namespace XBee.Frames
{
    public enum DeliveryStatus : byte
    {
        Success = 0x00,
        NoAck = 0x01,
        CcaFailure = 0x02,
        Purged = 0x03
    }
}
