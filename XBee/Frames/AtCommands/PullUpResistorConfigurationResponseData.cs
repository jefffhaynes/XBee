using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class PullUpResistorConfigurationResponseData : AtCommandResponseFrameData
    {
        [FieldOrder(0)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeSeries1, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeeProSeries1, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public PullUpResistorConfiguration? Configuration { get; set; }

        [FieldOrder(1)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBeePro900HP, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("ControllerHardwareVersion", HardwareVersion.XBee24C, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public PullUpResistorConfigurationExt? ConfigurationExt { get; set; }
    }
}
