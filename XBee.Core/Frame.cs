
using BinarySerialization;

namespace XBee
{
    public class Frame
    {
        public Frame()
        {
            StartDelimiter = StartDelimiter.FrameDelimiter;
        }

        public Frame(FrameContent content) : this()
        {
            Payload = new FramePayload(content);
        }

        [FieldOrder(0)]
        public StartDelimiter StartDelimiter { get; set; }

        [FieldOrder(1)]
        public ushort Length { get; set; }

        [FieldOrder(2)]
        [FieldLength("Length")]
        [ChecksumFieldValue("Checksum")]
        public FramePayload Payload { get; set; }

        [FieldOrder(3)]
        public byte Checksum { get; set; }
    }
}
