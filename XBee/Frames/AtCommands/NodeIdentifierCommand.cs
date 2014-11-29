
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NodeIdentifierCommand : AtCommand
    {
        public NodeIdentifierCommand()
            : base("NI")
        {
        }

        public NodeIdentifierCommand(string id) : this()
        {
            Id = id;
        }

        [Ignore]
        public string Id
        {
            get { return Parameter as string; }
            set { Parameter = value; }
        }
    }
}
