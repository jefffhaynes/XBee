using BinarySerialization;
using XBee.Converters;
using XBee.Devices;

namespace XBee.Frames.AtCommands
{
    internal class InputOutputChangeDetectionResponseData : AtCommandResponseFrameData
    {
        [FieldOrder(0)]
        [SerializeWhen("ControllerHardwareVersion", DeviceSeries.Series1, RelativeSourceMode = RelativeSourceMode.SerializationContext,
            ConverterType = typeof(VersionToSeriesConverter))]
        [SerializeAs(SerializedType.UInt1)]
        public DigitalSampleChannels? Channels { get; set; }

        [FieldOrder(1)]
        [SerializeWhen("ControllerHardwareVersion", DeviceSeries.Series2, RelativeSourceMode = RelativeSourceMode.SerializationContext,
            ConverterType = typeof(VersionToSeriesConverter))]
        [SerializeWhen("ControllerHardwareVersion", DeviceSeries.Pro900, RelativeSourceMode = RelativeSourceMode.SerializationContext,
            ConverterType = typeof(VersionToSeriesConverter))]
        public DigitalSampleChannels? ChannelsExt { get; set; }
    }
}
