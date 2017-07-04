namespace XBee.Frames.AtCommands
{
    internal class EndDeviceAssociationOptionsCommand : AtCommand
    {
        public EndDeviceAssociationOptionsCommand() : base("A1")
        {
        }

        public EndDeviceAssociationOptionsCommand(EndDeviceAssociationOptions options) : this()
        {
            Parameter = options;
        }
    }
}
