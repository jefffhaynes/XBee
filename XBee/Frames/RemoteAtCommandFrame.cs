namespace XBee.Frames
{
    public class RemoteAtCommandFrame : CommandFrameContent
    {
        public RemoteAtCommandFrame()
        {
        }

        public RemoteAtCommandFrame(NodeAddress address, AtCommandFrame command)
        {
            LongAddress = address.LongAddress;

            ShortAddress = address.ShortAddress;

            Options = RemoteAtCommandOptions.Commit;

            Command = command;
        }

        public LongAddress LongAddress { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public RemoteAtCommandOptions Options { get; set; }

        public AtCommandFrame Command { get; set; }
    }
}