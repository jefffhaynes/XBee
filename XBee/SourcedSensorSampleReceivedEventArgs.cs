using XBee.Frames;

namespace XBee
{
    public class SourcedSensorSampleReceivedEventArgs : SensorSampleReceivedEventArgs
    {
        internal SourcedSensorSampleReceivedEventArgs(NodeAddress address, OneWireSensor oneWireSensor,
            ushort sensorValueA, ushort sensorValueB, ushort sensorValueC,
            ushort sensorValueD, double temperatureCelcius)
            : base(oneWireSensor, sensorValueA, sensorValueB, sensorValueC, sensorValueD, temperatureCelcius)
        {
            Address = address;
        }

        public NodeAddress Address { get; }
    }
}