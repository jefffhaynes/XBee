namespace XBee
{
    public class SourcedSensorSample : Sourced
    {
        internal SourcedSensorSample(NodeAddress address, SensorSample sensorSample) : base(address)
        {
            SensorSample = sensorSample;
        }

        public SensorSample SensorSample { get; }
    }
}
