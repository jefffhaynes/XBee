using BinarySerialization;

namespace XBee.Frames
{
    internal class RxSmsFrame : FrameContent
    {
        [FieldOrder(0)]
        [FieldLength(20)]
        public string PhoneNumber { get; set; }

        [FieldOrder(1)]
        [FieldEncoding("utf-8")]
        public string Message { get; set; }
    }
}
