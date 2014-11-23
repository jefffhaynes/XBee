
using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee
{
    public class FrameSerializer
    {
        public HardwareVersion? CoordinatorHardwareVersion { get; set; }

        public void Serialize(Stream stream, object graph)
        {
            var serializer = CreateSerializer();
            serializer.Serialize(stream, graph);
        }

        public byte[] Serialize(Frame frame)
        {
            var serializer = CreateSerializer();
            var stream = new MemoryStream();

            serializer.Serialize(stream, frame,
                new BinarySerializationContext(new FrameContext(CoordinatorHardwareVersion)));

            var data = stream.ToArray();

            /* Calculate and append checksum */
            var crc = Crc.Calculate(data.Skip(3).ToArray());
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = crc;

            return data;
        }

        public Frame Deserialize(Stream stream)
        {
            var serializer = CreateSerializer();
            //serializer.MemberDeserializing += (sender, args) =>
            //{
            //    Console.CursorLeft = args.Context.Depth * 4;
            //    Console.WriteLine("Start: {0}", args.MemberName);
            //};

            //serializer.MemberDeserialized += (sender, args) =>
            //{
            //    Console.CursorLeft = args.Context.Depth * 4;
            //    var value = args.Value ?? "null";
            //    Console.WriteLine("End: {0} ({1})", args.MemberName, value);
            //};

            var frame =  serializer.Deserialize<Frame>(stream, 
                new BinarySerializationContext(new FrameContext(CoordinatorHardwareVersion)));

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
