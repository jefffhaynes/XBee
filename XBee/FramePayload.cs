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

        [FieldOrder(0)]
        public FrameType FrameType { get; set; }

        [FieldOrder(1)]
        [Subtype("FrameType", FrameType.ModemStatus, typeof(ModemStatusFrame))]
        [Subtype("FrameType", FrameType.ModemStatusExt, typeof(ModemStatusExtFrame))]
        [Subtype("FrameType", FrameType.AtCommand, typeof(AtCommandFrameContent))]
        [Subtype("FrameType", FrameType.AtCommandResponse, typeof(AtCommandResponseFrame))]
        [Subtype("FrameType", FrameType.RemoteAtCommand, typeof(RemoteAtCommandFrameContent))]
        [Subtype("FrameType", FrameType.RemoteAtCommandResponse, typeof(RemoteAtCommandResponseFrame))]
        [Subtype("FrameType", FrameType.TxRequest, typeof(TxRequestFrame))]
        [Subtype("FrameType", FrameType.TxRequest16, typeof(TxRequest16Frame))]
        [Subtype("FrameType", FrameType.TxRequestExt, typeof(TxRequestExtFrame))]
        [Subtype("FrameType", FrameType.TxStatus, typeof(TxStatusFrame))]
        [Subtype("FrameType", FrameType.TxStatusExt, typeof(TxStatusExtFrame))]
        [Subtype("FrameType", FrameType.RxIndicator, typeof(RxIndicatorFrame))]
        [Subtype("FrameType", FrameType.RxIndicator16, typeof(RxIndicator16Frame))]
        [Subtype("FrameType", FrameType.RxIndicatorExt, typeof(RxIndicatorExtFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorExplicitExt, typeof(RxIndicatorExplicitExtFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorSample, typeof(RxIndicatorSampleFrame))]
        [Subtype("FrameType", FrameType.RxIndicator16Sample, typeof(RxIndicator16SampleFrame))]
        [Subtype("FrameType", FrameType.RxIndicatorSampleExt, typeof(RxIndicatorSampleExtFrame))]
        [Subtype("FrameType", FrameType.SensorReadIndicator, typeof(SensorReadIndicatorFrame))]
        [Subtype("FrameType", FrameType.FirmwareUpdateStatus, typeof(FirmwareUpdateStatusFrame))]
        public FrameContent Content { get; set; }
    }
}
