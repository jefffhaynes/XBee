using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XBee.Frames;

namespace XBee
{
    public class SampleReceivedEventArgs : EventArgs
    {
        public SampleReceivedEventArgs(NodeAddress address, DigitalSampleState digitalSampleState, IEnumerable<AnalogSample> analogSamples)
        {
            Address = address;
            DigitalSampleState = digitalSampleState;
            AnalogSamples = new ReadOnlyCollection<AnalogSample>(analogSamples.ToList());
        }

        public NodeAddress Address { get; private set; }

        public DigitalSampleState DigitalSampleState { get; private set; }

        public ReadOnlyCollection<AnalogSample> AnalogSamples { get; private set; }

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
