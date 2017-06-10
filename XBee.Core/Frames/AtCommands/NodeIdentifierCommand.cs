
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class NodeIdentifierCommand : AtCommand
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
            get => Parameter as string;
            set => Parameter = value;
        }
    }
}
