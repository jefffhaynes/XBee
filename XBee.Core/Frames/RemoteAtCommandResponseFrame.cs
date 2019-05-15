using BinarySerialization;
using JetBrains.Annotations;

namespace XBee.Frames
{
    internal class RemoteAtCommandResponseFrame : CommandResponseFrameContent
    {
        [FieldOrder(0)] [UsedImplicitly] public LongAddress LongAddress { get; set; }

        [FieldOrder(1)] [UsedImplicitly] public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public AtCommandResponseFrameContent Content { get; set; }
    }
}
