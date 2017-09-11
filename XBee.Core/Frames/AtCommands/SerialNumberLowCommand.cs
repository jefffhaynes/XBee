namespace XBee.Frames.AtCommands
{
    internal class SerialNumberLowCommand : AtCommand
    {
        public const string Name = "SL";

        public SerialNumberLowCommand()
            : base(Name)
        {
        }
    }
}
