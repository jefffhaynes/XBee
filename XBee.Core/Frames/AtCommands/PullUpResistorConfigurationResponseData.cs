using BinarySerialization;
using XBee.Converters;
using XBee.Devices;

namespace XBee.Frames.AtCommands
{
    internal class PullUpResistorConfigurationResponseData : AtCommandResponseFrameData
    {
        [FieldOrder(0)]
        [SerializeWhen("ControllerHardwareVersion", DeviceSeries.Series1, RelativeSourceMode = RelativeSourceMode.SerializationContext,
            ConverterType = typeof(VersionToSeriesConverter))]
        public PullUpResistorConfiguration? Configuration { get; set; }

        [FieldOrder(1)]
        [SerializeWhen("ControllerHardwareVersion", DeviceSeries.Series2, RelativeSourceMode = RelativeSourceMode.SerializationContext,
            ConverterType = typeof(VersionToSeriesConverter))]
        [SerializeWhen("ControllerHardwareVersion", DeviceSeries.Pro900, RelativeSourceMode = RelativeSourceMode.SerializationContext,
            ConverterType = typeof(VersionToSeriesConverter))]
        public PullUpResistorConfigurationExt? ConfigurationExt { get; set; }
    }
}
