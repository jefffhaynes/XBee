namespace XBee.Frames
{
    public class TxStatusExtFrame : CommandResponseFrameContent
    {
        public TxStatusExtFrame()
        {
            Reserved = ShortAddress.Broadcast;
        }

        public ShortAddress Reserved { get; set; }

        public byte RetryCount { get; set; }

        public DeliveryStatusExt DeliveryStatus { get; set; }

        public DiscoveryStatus DiscoveryStatus { get; set; }
    }
}
