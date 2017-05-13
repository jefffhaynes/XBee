using System;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Frames
{
    public class TxIPv4Frame : CommandFrameContent
    {
        private const int MaxPayloadLength = 1500;

        public TxIPv4Frame()
        {
        }

        public TxIPv4Frame(uint address, ushort port, ushort sourcePort, InternetProtocol protocol, TxIPv4Options options, byte[] data)
        {
            if (data.Length > MaxPayloadLength)
                throw new ArgumentException($"Payload must be less than {MaxPayloadLength} bytes in length.", nameof(data));

            Address = address;
            Port = port;
            SourcePort = sourcePort;
            Protocol = protocol;
            Options = options;

            Data = data;
        }

        [FieldOrder(0)]
        public uint Address { get; set; }

        [FieldOrder(1)]
        public ushort Port { get; set; }

        [FieldOrder(2)]
        public ushort SourcePort { get; set; }

        [FieldOrder(3)]
        public InternetProtocol Protocol { get; set; }

        [FieldOrder(4)]
        public TxIPv4Options Options { get; set; }

        [FieldOrder(5)]
        public byte[] Data { get; set; }
    }
}
