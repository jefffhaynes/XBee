
using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee
{
    public class FrameSerializer
    {
        public HardwareVersion? ControllerHardwareVersion { get; set; }

        public void Serialize(Stream stream, object graph)
        {
            var serializer = CreateSerializer();
            serializer.Serialize(stream, graph);
        }

        public byte[] Serialize(Frame frame)
        {
            var serializer = CreateSerializer();
            var stream = new MemoryStream();

#if DEBUG
            serializer.MemberSerializing += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                Console.WriteLine("S-Start: {0}", args.MemberName);
            };

            serializer.MemberSerialized += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                var value = args.Value ?? "null";
                Console.WriteLine("S-End: {0} ({1})", args.MemberName, value);
            };
#endif

            serializer.Serialize(stream, frame,
                new BinarySerializationContext(new FrameContext(ControllerHardwareVersion)));

            var data = stream.ToArray();

            /* Calculate and append checksum */
            var crc = Checksum.Calculate(data.Skip(3).ToArray());
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = crc;

            return data;
        }

        public Frame Deserialize(Stream stream)
        {
            var serializer = CreateSerializer();

#if DEBUG
            serializer.MemberDeserializing += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                Console.WriteLine("D-Start: {0}", args.MemberName);
            };

            serializer.MemberDeserialized += (sender, args) =>
            {
                Console.CursorLeft = args.Context.Depth * 4;
                var value = args.Value ?? "null";
                Console.WriteLine("D-End: {0} ({1})", args.MemberName, value);
            };
#endif

            var frame =  serializer.Deserialize<Frame>(stream, 
                new BinarySerializationContext(new FrameContext(ControllerHardwareVersion)));

            /* read checksum */
            stream.ReadByte();

            return frame;
        }

        private BinarySerializer CreateSerializer()
        {
            return new BinarySerializer { Endianness = Endianness.Big };
        }
    }
}
