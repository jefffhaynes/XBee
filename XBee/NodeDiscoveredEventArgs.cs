using System;

namespace XBee
{
    public class NodeDiscoveredEventArgs : EventArgs
    {
        public NodeDiscoveredEventArgs(LongAddress address, string name, SignalStrength signalStrength)
        {
            Address = address;
            Name = name;
            SignalStrength = signalStrength;
        }

        public LongAddress Address { get; private set; }

        public string Name { get; private set; }

        public SignalStrength SignalStrength { get; private set; }
    }
}
