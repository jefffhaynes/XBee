namespace XBee.Frames.AtCommands
{
    internal class PullUpResistorConfigurationCommand : AtCommand
    {
        public PullUpResistorConfigurationCommand() : base("PR")
        {
        }

        public PullUpResistorConfigurationCommand(PullUpResistorConfiguration configuration) : this()
        {
            Parameter = configuration;
        }

        public PullUpResistorConfigurationCommand(PullUpResistorConfigurationExt configuration) : this()
        {
            Parameter = configuration;
        }
    }
}
