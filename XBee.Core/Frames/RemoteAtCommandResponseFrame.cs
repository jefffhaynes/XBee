using BinarySerialization;

namespace XBee.Frames
{
    public class RemoteAtCommandResponseFrame : CommandResponseFrameContent
    {
        [FieldOrder(0)]
        public LongAddress LongAddress { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public AtCommandResponseFrameContent Content { get; set; }
    }
}
