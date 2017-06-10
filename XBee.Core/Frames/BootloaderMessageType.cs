namespace XBee.Frames
{
    public enum BootloaderMessageType
    {
        Ack = 0x06,
        Nack = 0x15,
        NoMacAck = 0x40,
        Query = 0x51,
        QueryResponse = 0x52
    }
}
