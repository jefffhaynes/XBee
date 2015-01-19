using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee
{
    public class FrameSerializer
    {
        private readonly FrameContext _serializationContext = new FrameContext(null);
        private readonly BinarySerializer _serializer = new BinarySerializer {Endianness = Endianness.Big};

        public FrameSerializer()
        {
            _serializer.MemberSerializing += OnMemberSerializing;
            _serializer.MemberSerialized += OnMemberSerialized;
            _serializer.MemberDeserializing += OnMemberDeserializing;
            _serializer.MemberDeserialized += OnMemberDeserialized;
        }

        public HardwareVersion? ControllerHardwareVersion
        {
            get { return _serializationContext.ControllerHardwareVersion; }
            set { _serializationContext.ControllerHardwareVersion = value; }
        }

        /// <summary>
        ///     Occurrs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        ///     Occurrs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        ///     Occurrs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        ///     Occurrs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

        public byte[] Serialize(Frame frame)
        {
            var stream = new MemoryStream();

            _serializer.Serialize(stream, frame, _serializationContext);

            byte[] data = stream.ToArray();

            /* Calculate and append checksum */
            byte crc = Checksum.Calculate(data.Skip(3).ToArray());
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = crc;

            return data;
        }

        public Frame Deserialize(Stream stream)
        {
            var frame = _serializer.Deserialize<Frame>(stream, _serializationContext);

            /* read checksum */
            stream.ReadByte();

            return frame;
        }

        private void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberSerialized;
            if (handler != null)
                handler(sender, e);
        }

        private void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberDeserialized;
            if (handler != null)
                handler(sender, e);
        }

        private void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberSerializing;
            if (handler != null)
                handler(sender, e);
        }

        private void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberDeserializing;
            if (handler != null)
                handler(sender, e);
        }
    }
}