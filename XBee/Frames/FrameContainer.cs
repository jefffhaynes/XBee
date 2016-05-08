using BinarySerialization;

namespace XBee.Frames
{
    public class FrameContainer
    {
        public FrameContainer(Frame frame)
        {
            Frame = frame;
        }

        [FieldOrder(0)]
        [ChecksumFieldValue("Checksum")]
        public Frame Frame { get; set; }

        [FieldOrder(1)]
        public byte Checksum { get; set; }
    }
}
