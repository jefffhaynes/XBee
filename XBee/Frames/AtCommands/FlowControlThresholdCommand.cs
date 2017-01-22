namespace XBee.Frames.AtCommands
{
    public class FlowControlThresholdCommand : AtCommand
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
