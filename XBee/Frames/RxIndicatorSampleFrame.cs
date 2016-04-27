using BinarySerialization;

namespace XBee.Frames
{
    public class RxIndicatorSampleFrame : FrameContent, IRxIndicatorSampleFrame
    {
        [FieldOrder(0)]
        public LongAddress Source { get; set; }

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