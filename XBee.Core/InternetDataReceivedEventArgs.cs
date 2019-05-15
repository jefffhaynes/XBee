using System;
using System.Net;
using JetBrains.Annotations;
using XBee.Frames.AtCommands;

namespace XBee
{
    [PublicAPI]
    public class InternetDataReceivedEventArgs : EventArgs
    {
        internal InternetDataReceivedEventArgs(IPAddress sourceAddress, ushort destinationPort, ushort sourcePort,
            InternetProtocol protocol, byte[] data)
        {
            SourceAddress = sourceAddress;
            DestinationPort = destinationPort;
            SourcePort = sourcePort;
            Protocol = protocol;
            Data = data;
        }

        public IPAddress SourceAddress { get; }

        public ushort DestinationPort { get; }

        public ushort SourcePort { get; }

        public InternetProtocol Protocol { get; }

        public byte[] Data { get; }
    }
}
