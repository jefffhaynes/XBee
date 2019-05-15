using System.Collections.Generic;
using JetBrains.Annotations;
using XBee.Frames;

namespace XBee
{
    [PublicAPI]
    public class SourcedSampleReceivedEventArgs : SampleReceivedEventArgs
    {
        internal SourcedSampleReceivedEventArgs(NodeAddress address, DigitalSampleChannels digitalChannels, DigitalSampleState digitalSampleState,
            AnalogSampleChannels analogChannels, IEnumerable<AnalogSample> analogSamples)
            : base(digitalChannels, digitalSampleState, analogChannels, analogSamples)
        {
            Address = address;
        }

        public NodeAddress Address { get; }
    }
}
