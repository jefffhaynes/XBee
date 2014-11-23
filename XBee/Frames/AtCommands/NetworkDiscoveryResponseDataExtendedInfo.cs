namespace XBee.Frames.AtCommands
{
    public class NetworkDiscoveryResponseDataExtendedInfo
    {
        public ShortAddress ParentNetworkAddress { get; set; }

        public DeviceType DeviceType { get; set; }

        public byte ReservedStatus { get; set; }

        public ushort ProfileId { get; set; }

        public ushort ManufactureId { get; set; }

        public byte[] OptionalData { get; set; }
    }
}
