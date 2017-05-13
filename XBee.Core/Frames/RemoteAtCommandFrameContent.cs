using BinarySerialization;

namespace XBee.Frames
{
    public class RemoteAtCommandFrameContent : CommandFrameContent
    {
        public RemoteAtCommandFrameContent()
        {
        }

        public RemoteAtCommandFrameContent(NodeAddress address, AtCommand command)
        {
            LongAddress = address.LongAddress;

            ShortAddress = address.ShortAddress;

            Options = RemoteAtCommandOptions.Commit;

            Command = command;
        }

        [FieldOrder(0)]
        public LongAddress LongAddress { get; set; }

        [FieldOrder(1)]
        public ShortAddress ShortAddress { get; set; }

        [FieldOrder(2)]
        public RemoteAtCommandOptions Options { get; set; }

        [FieldOrder(3)]
        public AtCommand Command { get; set; }
    }
}