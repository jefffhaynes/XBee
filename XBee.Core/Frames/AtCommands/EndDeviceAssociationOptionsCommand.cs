namespace XBee.Frames.AtCommands
{
    internal class EndDeviceAssociationOptionsCommand : AtCommand
    {
        public const string Name = "A1";

        public EndDeviceAssociationOptionsCommand() : base(Name)
        {
        }

        public EndDeviceAssociationOptionsCommand(EndDeviceAssociationOptions options) : this()
        {
            Parameter = options;
        }
    }
}
