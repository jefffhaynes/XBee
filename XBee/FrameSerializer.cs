
using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee
{
    public class FrameSerializer
    {
        private readonly BinarySerializer _serializer = new BinarySerializer { Endianness = Endianness.Big };
        private readonly BinarySerializer _deserializer = new BinarySerializer { Endianness = Endianness.Big };

#if DEBUG
        public FrameSerializer()
        {
            _serializer.MemberSerializing += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                Console.WriteLine("S-Start: {0}", args.MemberName);
            };

            _serializer.MemberSerialized += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                var value = args.Value ?? "null";
                Console.WriteLine("S-End: {0} ({1})", args.MemberName, value);
            };

            _deserializer.MemberDeserializing += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                Console.WriteLine("D-Start: {0}", args.MemberName);
            };

            _deserializer.MemberDeserialized += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                var value = args.Value ?? "null";
                Console.WriteLine("D-End: {0} ({1})", args.MemberName, value);
            };
        }
#endif

        public HardwareVersion? ControllerHardwareVersion
        {
            get { return _serializationContext.ControllerHardwareVersion; }
            set { _serializationContext.ControllerHardwareVersion = value; }
        }

        private readonly FrameContext _serializationContext = new FrameContext(null);

        public void Serialize(Stream stream, object graph)
        {
            _serializer.Serialize(stream, graph);
        }

        public byte[] Serialize(Frame frame)
        {
            var stream = new MemoryStream();

            _serializer.Serialize(stream, frame, _serializationContext);

            var data = stream.ToArray();

            /* Calculate and append checksum */
            var crc = Checksum.Calculate(data.Skip(3).ToArray());
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = crc;

            return data;
        }

        public Frame Deserialize(Stream stream)
        {
            var frame = _deserializer.Deserialize<Frame>(stream, _serializationContext);

            /* read checksum */
            stream.ReadByte();

            return frame;
        }
    }
}
