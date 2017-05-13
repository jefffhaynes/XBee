using System;

namespace XBee
{
    public class NodeDiscoveredEventArgs : EventArgs
    {
        internal NodeDiscoveredEventArgs(string name, SignalStrength? signalStrength, XBeeNode node)
        {
            Name = name;
            SignalStrength = signalStrength;
            Node = node;
        }

        public string Name { get; }

        public SignalStrength? SignalStrength { get; }

        public XBeeNode Node { get; }
    }
}
