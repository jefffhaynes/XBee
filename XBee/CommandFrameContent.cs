using BinarySerialization;

namespace XBee
{
    public abstract class CommandFrameContent : FrameContent
    {
        [SerializeAs(Order = int.MinValue)]
        public byte FrameId { get; set; }
    }
}
