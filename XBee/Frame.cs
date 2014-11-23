
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

        public StartDelimiter StartDelimiter { get; set; }

        public ushort Length { get; set; }

        [FieldLength("Length")]
        public FramePayload Payload { get; set; }
    }
}
