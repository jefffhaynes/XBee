using XBee.Frames;

namespace XBee
{
    public class SensorSample
    {
        internal SensorSample(OneWireSensor oneWireSensor, ushort sensorValueA, ushort sensorValueB, ushort sensorValueC,
            ushort sensorValueD, double degreesCelsius)
        {
            OneWireSensor = oneWireSensor;
            SensorValueA = sensorValueA;
            SensorValueB = sensorValueB;
            SensorValueC = sensorValueC;
            SensorValueD = sensorValueD;
            TemperatureCelsius = degreesCelsius;
        }
        
        public OneWireSensor OneWireSensor { get; }
        
        public ushort SensorValueA { get; }
        
        public ushort SensorValueB { get; }
        
        public ushort SensorValueC { get; }
        
        public ushort SensorValueD { get; }

        public double TemperatureCelsius { get; }
    }
}
