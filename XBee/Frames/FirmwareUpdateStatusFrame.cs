using BinarySerialization;

namespace XBee.Frames
{
    public class FirmwareUpdateStatusFrame : CommandResponseFrameContent
    {
        [FieldOrder(0)]
        public LongAddress LongAddress { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public ReceiveOptions ReceiveOptions { get; set; }

        [FieldOrder(3)]
        public BootloaderMessageType BootloaderMessageType { get; set; }

        [FieldOrder(4)]
        public byte BlockNumber { get; set; }

        [FieldOrder(5)]
        public LongAddress TargetAddress { get; set; }
    }
}
