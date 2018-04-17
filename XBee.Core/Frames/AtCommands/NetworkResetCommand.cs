namespace XBee.Frames.AtCommands
{
    internal class NetworkResetCommand : AtCommand
    {
        public const string Name = "NR";

        public NetworkResetCommand() : base(Name)
        {
        }

        public NetworkResetCommand(NetworkResetOptions option) : this()
        {
            Parameter = option;
        }
    }
}