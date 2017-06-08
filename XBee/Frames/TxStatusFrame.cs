namespace XBee.Frames
{
    internal class TxStatusFrame : CommandResponseFrameContent
    {
        public DeliveryStatus Status { get; set; }
    }
}
