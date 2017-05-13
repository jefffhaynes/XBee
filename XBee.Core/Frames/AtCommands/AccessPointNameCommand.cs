namespace XBee.Frames.AtCommands
{
    public class AccessPointNameCommand : AtCommand
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
