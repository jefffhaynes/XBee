using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class HardwareVersionResponseData : AtCommandResponseFrameData
    {
        [FieldOrder(0)]
        public HardwareVersion HardwareVersion { get; set; }

        [FieldOrder(1)]
        public byte HardwareRevision { get; set; }
    }
}
