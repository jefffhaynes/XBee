namespace XBee.Frames.AtCommands
{
    internal class DeviceTypeIdentifierCommand : AtCommand
    {
        public const string Name = "DD";

        public DeviceTypeIdentifierCommand() : base(Name)
        {
        }

        public DeviceTypeIdentifierCommand(uint type) : this()
        {
            Parameter = type;
        }
    }
}