using System;

namespace XBee
{
    public class NodeDiscoveredEventArgs : EventArgs
    {
        public NodeDiscoveredEventArgs(string name, SignalStrength? signalStrength, XBeeNode node)
        {
            Name = name;
            SignalStrength = signalStrength;
            Node = node;
        }

        public string Name { get; private set; }

        public SignalStrength? SignalStrength { get; private set; }

        public XBeeNode Node { get; private set; }
    }
}
