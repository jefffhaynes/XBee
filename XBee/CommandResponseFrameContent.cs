using BinarySerialization;

namespace XBee
{
    public abstract class CommandResponseFrameContent : FrameContent
    {
        [SerializeAs(Order = int.MinValue)]
        public byte FrameId { get; set; }
    }
}
