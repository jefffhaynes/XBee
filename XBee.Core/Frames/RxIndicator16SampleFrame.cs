using BinarySerialization;

namespace XBee.Frames
{
    public class RxIndicator16SampleFrame : FrameContent, IRxIndicatorSampleFrame
    {
        [FieldOrder(0)]
        public ShortAddress Source { get; set; }

        [FieldOrder(1)]
        public RxIndicatorSampleFrameContent Content { get; set; }

        public Sample GetSample()
        {
            return Content.GetSample();
        }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source);
        }
    }
}