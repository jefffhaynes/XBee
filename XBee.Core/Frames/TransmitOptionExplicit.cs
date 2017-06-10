namespace XBee.Frames
{
    public enum TransmitOptionExplicit : byte
    {
        DisableRetriesAndRouteRepair = 0x01,
        EnableApsEncryption = 0x20,
        UseExtendedTransmissionTimeout = 0x40
    }
}
