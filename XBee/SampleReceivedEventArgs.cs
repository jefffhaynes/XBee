using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XBee.Frames;

namespace XBee
{
    public class SampleReceivedEventArgs : EventArgs
    {
        public SampleReceivedEventArgs(DigitalSampleState digitalSampleState, List<AnalogSample> analogSamples)
        {
            DigitalSampleState = digitalSampleState;
            AnalogSamples = analogSamples;
        }

        public DigitalSampleState DigitalSampleState { get; set; }

        public List<AnalogSample> AnalogSamples { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder(string.Format("DIO: {0}", DigitalSampleState));

            if (AnalogSamples.Any())
            {
                var analogSamples = string.Join(", ", AnalogSamples);
                builder.AppendFormat(", ADC: {0}", analogSamples);
            }

            return builder.ToString();
        }
    }
}
