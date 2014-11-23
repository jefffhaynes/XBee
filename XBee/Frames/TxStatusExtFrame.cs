namespace XBee.Frames
{
    public class TxStatusExtFrame : CommandResponseFrameContent
    {
        private static readonly byte[] ReservedData = { 0xff, 0xfe };

        public TxStatusExtFrame()
        {
            Reserved = ReservedData;
        }

        public byte[] Reserved { get; set; }

        public byte RetryCount { get; set; }

        public DeliveryStatusExt DeliveryStatus { get; set; }

        public DiscoveryStatus DiscoveryStatus { get; set; }
    }
}
