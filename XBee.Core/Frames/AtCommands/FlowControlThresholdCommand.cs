namespace XBee.Frames.AtCommands
{
    internal class FlowControlThresholdCommand : AtCommand
    {
        public const string Name = "FT";

        public FlowControlThresholdCommand() : base(Name)
        {
        }

        public FlowControlThresholdCommand(byte threshold) : this()
        {
            Parameter = threshold;
        }
    }
}
