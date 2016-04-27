using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XBee.Frames;

namespace XBee
{
    public class Sample
    {
        internal Sample(DigitalSampleChannels digitalChannels, DigitalSampleState digitalSampleState, 
            AnalogSampleChannels analogChannels, IEnumerable<AnalogSample> analogSamples)
        {
            DigitalChannels = digitalChannels;
            DigitalSampleState = digitalSampleState;
            AnalogChannels = analogChannels;
            AnalogSamples = new ReadOnlyCollection<AnalogSample>(analogSamples.ToList());
        }

        public DigitalSampleChannels DigitalChannels { get; }

        public DigitalSampleState DigitalSampleState { get; }

        public AnalogSampleChannels AnalogChannels { get; }

        public ReadOnlyCollection<AnalogSample> AnalogSamples { get; }

        public override string ToString()
        {
            var builder = new StringBuilder($"DIO: {DigitalSampleState}");

            if (AnalogSamples.Any())
            {
                var analogSamples = string.Join(", ", AnalogSamples);
                builder.AppendFormat(", ADC: {0}", analogSamples);
            }

            return builder.ToString();
        }
    }
}
