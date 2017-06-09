namespace XBee.Frames.AtCommands
{
    internal class AccessPointNameCommand : AtCommand
    {
        public AccessPointNameCommand() : base("AN")
        {
        }

        public AccessPointNameCommand(string name) : this()
        {
            Parameter = name;
        }
    }
}
