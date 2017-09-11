namespace XBee.Frames.AtCommands
{
    internal class PullUpResistorConfigurationCommand : AtCommand
    {
        public const string Name = "PR";

        public PullUpResistorConfigurationCommand() : base(Name)
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
