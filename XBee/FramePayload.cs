using BinarySerialization;
using XBee.Frames;

namespace XBee
{
    public class FramePayload
    {
        public FramePayload()
        {
        }

        public FramePayload(FrameContent content)
        {
            Content = content;
        }

        public FrameType FrameType { get; set; }

        [Subtype("FrameType", FrameType.AtCommand, typeof(AtCommandFrame))]
        [Subtype("FrameType", FrameType.AtCommandResponse, typeof(AtCommandResponseFrame))]
        [Subtype("FrameType", FrameType.TxRequest, typeof(TxRequestFrame))]
        [Subtype("FrameType", FrameType.TxRequestExt, typeof(TxRequestExtFrame))]
        [Subtype("FrameType", FrameType.TxStatus, typeof(TxStatusFrame))]
        [Subtype("FrameType", FrameType.TxStatusExt, typeof(TxStatusExtFrame))]
        [Subtype("FrameType", FrameType.ModemStatus, typeof(ModemStatusFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorSample, typeof(RxIndicatorSampleFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorExt, typeof(RxIndicatorExtFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorExplicitExt, typeof(RxIndicatorExplicitExtFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorSampleExt, typeof(RxIndicatorSampleExtFrame))]
        [Subtype("FrameType", FrameType.RemoteAtCommand, typeof(RemoteAtCommandFrame))]
        public FrameContent Content { get; set; }
    }
}
