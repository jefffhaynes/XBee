using BinarySerialization;
using JetBrains.Annotations;
using XBee.Frames;

namespace XBee
{
    internal class FramePayload
    {
        [UsedImplicitly]
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
        [Subtype(nameof(FrameType), FrameType.ModemStatus, typeof(ModemStatusFrame))]
        [Subtype(nameof(FrameType), FrameType.ModemStatusExt, typeof(ModemStatusExtFrame))]
        [Subtype(nameof(FrameType), FrameType.AtCommand, typeof(AtCommandFrameContent))]
        [Subtype(nameof(FrameType), FrameType.AtQueuedCommand, typeof(AtQueuedCommandFrameContent))]
        [Subtype(nameof(FrameType), FrameType.AtCommandResponse, typeof(AtCommandResponseFrame))]
        [Subtype(nameof(FrameType), FrameType.RemoteAtCommand, typeof(RemoteAtCommandFrameContent))]
        [Subtype(nameof(FrameType), FrameType.RemoteAtCommandResponse, typeof(RemoteAtCommandResponseFrame))]
        [Subtype(nameof(FrameType), FrameType.TxRequest, typeof(TxRequestFrame))]
        [Subtype(nameof(FrameType), FrameType.TxRequest16, typeof(TxRequest16Frame))]
        [Subtype(nameof(FrameType), FrameType.TxRequestExt, typeof(TxRequestExtFrame))]
        [Subtype(nameof(FrameType), FrameType.TxRequestExplicit, typeof(TxRequestExplicitFrame))]
        [Subtype(nameof(FrameType), FrameType.TxStatus, typeof(TxStatusFrame))]
        [Subtype(nameof(FrameType), FrameType.TxStatusExt, typeof(TxStatusExtFrame))]
        [Subtype(nameof(FrameType), FrameType.TxSms, typeof(TxSmsFrame))]
        [Subtype(nameof(FrameType), FrameType.TxIPv4, typeof(TxIPv4Frame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicator, typeof(RxIndicatorFrame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicator16, typeof(RxIndicator16Frame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicatorExt, typeof(RxIndicatorExtFrame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicatorExplicitExt, typeof(RxIndicatorExplicitExtFrame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicatorSample, typeof(RxIndicatorSampleFrame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicator16Sample, typeof(RxIndicator16SampleFrame))]
        [Subtype(nameof(FrameType), FrameType.RxIndicatorSampleExt, typeof(RxIndicatorSampleExtFrame))]
        [Subtype(nameof(FrameType), FrameType.RxSms, typeof(RxSmsFrame))]
        [Subtype(nameof(FrameType), FrameType.RxIPv4, typeof(RxIPv4Frame))]
        [Subtype(nameof(FrameType), FrameType.SensorReadIndicator, typeof(SensorReadIndicatorFrame))]
        [Subtype(nameof(FrameType), FrameType.NodeIdentificationIndicator, typeof(NodeIdentificationFrame))]
        [Subtype(nameof(FrameType), FrameType.FirmwareUpdateStatus, typeof(FirmwareUpdateStatusFrame))]
        public FrameContent Content { get; set; }
    }
}
