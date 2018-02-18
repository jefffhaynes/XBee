namespace XBee.Frames.AtCommands
{
    internal class SignalStrengthCommand : AtCommand
    {
        public const string Name = "DB";

        public SignalStrengthCommand() : base(Name)
        {
        }
    }
}
