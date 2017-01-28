using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Frames
{
    public class RxIPv4Frame : CommandFrameContent
    {
        [FieldOrder(0)]
        public uint SourceAddress { get; set; }

        [FieldOrder(1)]
        public ushort DestinationPort { get; set; }

        [FieldOrder(2)]
        public ushort SourcePort { get; set; }

        [FieldOrder(3)]
        public InternetProtocol Protocol { get; set; }

        [FieldOrder(4)]
        public byte Status { get; set; }

        [FieldOrder(5)]
        public byte[] Data { get; set; }
    }
}
