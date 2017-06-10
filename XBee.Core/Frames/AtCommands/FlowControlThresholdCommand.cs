namespace XBee.Frames.AtCommands
{
    internal class FlowControlThresholdCommand : AtCommand
    {
        public FlowControlThresholdCommand() : base("FT")
        {
        }

        public FlowControlThresholdCommand(byte threshold) : this()
        {
            Parameter = threshold;
        }
    }
}
