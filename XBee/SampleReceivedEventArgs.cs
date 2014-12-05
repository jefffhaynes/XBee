using System;
using System.Collections.Generic;
using XBee.Frames;

namespace XBee
{
    public class SampleReceivedEventArgs : EventArgs
    {
        public SampleReceivedEventArgs(NodeAddress address, DigitalSampleState digitalSampleState, List<AnalogSample> analogSamples)
        {
            Address = address;
            DigitalSampleState = digitalSampleState;
            AnalogSamples = analogSamples;
        }

        public NodeAddress Address { get; private set; }

        public DigitalSampleState DigitalSampleState { get; private set; }

        public List<AnalogSample> AnalogSamples { get; private set; } 
    }
}
