namespace XBee.Frames.AtCommands
{
    internal class MaximumPayloadBytesCommand : AtCommand
    {
        public const string Name = "NP";

        public MaximumPayloadBytesCommand() : base(Name)
        {
        }
    }
}
