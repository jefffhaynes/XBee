using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NetworkDiscoveryResponseDataExtendedInfo
    {
        [FieldOrder(0)]
        public ShortAddress ParentNetworkAddress { get; set; }

        [FieldOrder(1)]
        public DeviceType DeviceType { get; set; }

        [FieldOrder(2)]
        public byte ReservedStatus { get; set; }

        [FieldOrder(3)]
        public ushort ProfileId { get; set; }

        [FieldOrder(4)]
        public ushort ManufactureId { get; set; }

        [FieldOrder(5)]
        public byte[] OptionalData { get; set; }
    }
}
