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

        public LongAddress LongAddress { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public RemoteAtCommandOptions Options { get; set; }

        public AtCommand Command { get; set; }
    }
}