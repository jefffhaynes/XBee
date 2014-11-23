namespace XBee.Frames
{
    public class TxStatusFrame : CommandResponseFrameContent
    {
        public DeliveryStatus Status { get; set; }
    }
}
