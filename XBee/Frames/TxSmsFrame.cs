using System;
using BinarySerialization;

namespace XBee.Frames
{
    public class TxSmsFrame : CommandFrameContent
    {
        private const int MaxPayloadLength = 140;

        public TxSmsFrame()
        {
        }

        public TxSmsFrame(string phoneNumber, string message)
        {
            PhoneNumber = phoneNumber;

            var data = System.Text.Encoding.UTF8.GetBytes(message);
            if (data.Length > MaxPayloadLength)
                throw new ArgumentException("Message too long", nameof(message));

            Data = data;
        }

        [FieldOrder(0)]
        public byte Options { get; set; }

        [FieldOrder(1)]
        [FieldLength(20)]
        public string PhoneNumber { get; set; }

        [FieldOrder(2)]
        public byte[] Data { get; set; }
    }
}
