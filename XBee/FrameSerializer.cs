using System;
using System.IO;
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
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

        public byte[] Serialize(Frame frame)
        {
            var stream = new MemoryStream();
            _serializer.Serialize(stream, frame, _serializationContext);
            return stream.ToArray();
        }

        public Frame Deserialize(Stream stream)
        {
            return _serializer.Deserialize<Frame>(stream, _serializationContext);
        }

        private void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberSerialized;
            handler?.Invoke(sender, e);
        }

        private void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberDeserialized;
            handler?.Invoke(sender, e);
        }

        private void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberSerializing;
            handler?.Invoke(sender, e);
        }

        private void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberDeserializing;
            handler?.Invoke(sender, e);
        }
    }
}