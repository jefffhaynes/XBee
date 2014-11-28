namespace XBee.Frames
{
    public class RemoteAtCommandFrame : CommandFrameContent
    {
        public RemoteAtCommandFrame()
        {
        }

        public RemoteAtCommandFrame(LongAddress destination, AtCommandFrame command)
        {
            Destination = destination;

            Options = RemoteAtCommandOptions.Commit;

            Command = command;
        }

        public LongAddress Destination { get; set; }

        public ShortAddress ShortAddress { get; set; }

        public RemoteAtCommandOptions Options { get; set; }

        public AtCommandFrame Command { get; set; }
    }
}
