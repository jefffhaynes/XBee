namespace XBee.Frames.AtCommands
{
    internal class ForceDisassociationCommand : AtCommand
    {
        public const string Name = "DA";

        public ForceDisassociationCommand() : base(Name)
        {
        }
    }
}
