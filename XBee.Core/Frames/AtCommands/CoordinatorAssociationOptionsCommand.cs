namespace XBee.Frames.AtCommands
{
    internal class CoordinatorAssociationOptionsCommand : AtCommand
    {
        public CoordinatorAssociationOptionsCommand() : base("A2")
        {
        }

        public CoordinatorAssociationOptionsCommand(CoordinatorAssociationOptions options) : this()
        {
            Parameter = options;
        }
    }
}
