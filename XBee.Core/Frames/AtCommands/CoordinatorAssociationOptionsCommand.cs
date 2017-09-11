namespace XBee.Frames.AtCommands
{
    internal class CoordinatorAssociationOptionsCommand : AtCommand
    {
        public const string Name = "A2";

        public CoordinatorAssociationOptionsCommand() : base(Name)
        {
        }

        public CoordinatorAssociationOptionsCommand(CoordinatorAssociationOptions options) : this()
        {
            Parameter = options;
        }
    }
}
