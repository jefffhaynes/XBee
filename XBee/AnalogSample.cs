using System;
using BinarySerialization;
using XBee.Frames;

namespace XBee
{
    public class AnalogSample
    {
        internal AnalogSample(SampleChannels sampleChannel, ushort value)
        {
            switch (sampleChannel)
            {
                case SampleChannels.Analog0:
                    Channel = 0;
                    break;
                case SampleChannels.Analog1:
                    Channel = 1;
                    break;
                case SampleChannels.Analog2:
                    Channel = 2;
                    break;
                case SampleChannels.Analog3:
                    Channel = 3;
                    break;
                case SampleChannels.Analog4:
                    Channel = 4;
                    break;
                case SampleChannels.Analog5:
                    Channel = 5;
                    break;
                default:
                    throw new InvalidOperationException("Unknown channel");
            }
            Value = value;
        }

        internal AnalogSample(AnalogSampleChannels sampleChannel, ushort value)
        {
            switch (sampleChannel)
            {
                case AnalogSampleChannels.Input0:
                    Channel = 0;
                    break;
                case AnalogSampleChannels.Input1:
                    Channel = 1;
                    break;
                case AnalogSampleChannels.Input2:
                    Channel = 2;
                    break;
                case AnalogSampleChannels.Input3:
                    Channel = 3;
                    break;
                default:
                    throw new InvalidOperationException("Unknown channel.");
            }
            Value = value;
        }

        [Ignore]
        public int Channel { get; }

        public ushort Value { get; set; }

        public override string ToString()
        {
            return $"{Channel}: {Value}";
        }
    }
}
