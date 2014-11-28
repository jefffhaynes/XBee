using System;
using System.Collections.Generic;
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
    }
}
