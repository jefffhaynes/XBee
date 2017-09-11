namespace XBee.Frames.AtCommands
{
    internal class AccessPointNameCommand : AtCommand
    {
        public const string Name = "AN";

        public AccessPointNameCommand() : base(Name)
        {
        }

        public AccessPointNameCommand(string name) : this()
        {
            Parameter = name;
        }
    }
}
