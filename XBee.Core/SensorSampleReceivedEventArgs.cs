using System;
using XBee.Frames;

namespace XBee
{
    public class SensorSampleReceivedEventArgs : EventArgs
    {
        internal SensorSampleReceivedEventArgs(OneWireSensor oneWireSensor, ushort sensorValueA, ushort sensorValueB, ushort sensorValueC,
            ushort sensorValueD, double temperatureCelsius)
        {
            OneWireSensor = oneWireSensor;
            SensorValueA = sensorValueA;
            SensorValueB = sensorValueB;
            SensorValueC = sensorValueC;
            SensorValueD = sensorValueD;
            TemperatureCelsius = temperatureCelsius;
        }

        public OneWireSensor OneWireSensor { get; }

        public ushort SensorValueA { get; }

        public ushort SensorValueB { get; }

        public ushort SensorValueC { get; }

        public ushort SensorValueD { get; }

        public double TemperatureCelsius { get; }
    }
}
