using XBee.Frames;

namespace XBee
{
    public class SensorSample
    {
        internal SensorSample(OneWireSensor oneWireSensor, ushort sensorValueA, ushort sensorValueB, ushort sensorValueC,
            ushort sensorValueD, double temperatureCelcius)
        {
            OneWireSensor = oneWireSensor;
            SensorValueA = sensorValueA;
            SensorValueB = sensorValueB;
            SensorValueC = sensorValueC;
            SensorValueD = sensorValueD;
            TemperatureCelsius = temperatureCelcius;
        }
        
        public OneWireSensor OneWireSensor { get; }
        
        public ushort SensorValueA { get; }
        
        public ushort SensorValueB { get; }
        
        public ushort SensorValueC { get; }
        
        public ushort SensorValueD { get; }

        public double TemperatureCelsius { get; }
    }
}
