namespace XBee.Frames.AtCommands
{
    internal class PullUpResistorConfigurationCommand : AtCommand
    {
        public PullUpResistorConfigurationCommand() : base("PR")
        {
        }

        public PullUpResistorConfigurationCommand(PullUpResistorConfiguration configuration)
        {
            Parameter = configuration;
        }

        public PullUpResistorConfigurationCommand(PullUpResistorConfigurationExt configuration)
        {
            Parameter = configuration;
        }
    }
}
