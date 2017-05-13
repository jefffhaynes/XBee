using BinarySerialization;

namespace XBee.Frames
{
    public class SensorReadIndicatorFrame : FrameContent, IRxIndicatorFrame
    {
        [FieldOrder(0)]
        public LongAddress Source { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public ReceiveOptionsExt ReceiveOptions { get; set; }

        [FieldOrder(3)]
        public OneWireSensor OneWireSensor { get; set; }

        [FieldOrder(4)]
        public ushort SensorValueA { get; set; }

        [FieldOrder(5)]
        public ushort SensorValueB { get; set; }

        [FieldOrder(6)]
        public ushort SensorValueC { get; set; }

        [FieldOrder(7)]
        public ushort SensorValueD { get; set; }

        [FieldOrder(8)]
        public ushort TemperatureValue { get; set; }

        [Ignore]
        public double TemperatureCelsius
        {
            get
            {
                if (TemperatureValue < 2048)
                    return (double) TemperatureValue/16;
                return -(double) (TemperatureValue & 0x7FF)/16;
            }
        }

        public NodeAddress GetAddress()
        {
            return new NodeAddress(Source, ShortAddress);
        }
    }
}
