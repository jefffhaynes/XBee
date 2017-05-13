using BinarySerialization;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee
{
    public class NodeIdentificationFrame : FrameContent
    {
        [FieldOrder(0)]
        public LongAddress SenderLongAddress { get; set; }

        [FieldOrder(1)]
        public ShortAddress SenderShortAddress { get; set; }

        [FieldOrder(2)]
        public ReceiveOptionsExt ReceiveOptions { get; set; }
        
        [FieldOrder(3)]
        public ShortAddress RemoteShortAddress { get; set; }

        [FieldOrder(4)]
        public LongAddress RemoteLongAddress { get; set; }

        [FieldOrder(5)]
        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Name { get; set; }

        [FieldOrder(6)]
        public ShortAddress ParentAddress { get; set; }

        [FieldOrder(7)]
        public DeviceType DeviceType { get; set; }

        [FieldOrder(8)]
        public NodeIdentificationReason NodeIdentificationReason { get; set; }

        [FieldOrder(9)]
        public ushort DigiProfileId { get; set; }

        [FieldOrder(10)]
        public ushort ManufacturerId { get; set; }
    }
}
