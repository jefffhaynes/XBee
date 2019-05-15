using JetBrains.Annotations;
using XBee.Frames;

namespace XBee
{
    [PublicAPI]
    public class SourcedSensorSampleReceivedEventArgs : SensorSampleReceivedEventArgs
    {
        internal SourcedSensorSampleReceivedEventArgs(NodeAddress address, OneWireSensor oneWireSensor,
            ushort sensorValueA, ushort sensorValueB, ushort sensorValueC,
            ushort sensorValueD, double temperatureCelsius)
            : base(oneWireSensor, sensorValueA, sensorValueB, sensorValueC, sensorValueD, temperatureCelsius)
        {
            Address = address;
        }

        public NodeAddress Address { get; }
    }
}