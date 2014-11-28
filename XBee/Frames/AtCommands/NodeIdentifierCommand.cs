
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NodeIdentifierCommand : AtCommandFrame
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
