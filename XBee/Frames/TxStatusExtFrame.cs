using BinarySerialization;

namespace XBee.Frames
{
    public class TxStatusExtFrame : CommandResponseFrameContent
    {
        public TxStatusExtFrame()
        {
            Reserved = ShortAddress.Broadcast;
        }

        [FieldOrder(0)]
        public ShortAddress Reserved { get; set; }

        [FieldOrder(1)]
        public byte RetryCount { get; set; }

        [FieldOrder(2)]
        public DeliveryStatusExt DeliveryStatus { get; set; }

        [FieldOrder(3)]
        public DiscoveryStatus DiscoveryStatus { get; set; }
    }
}
