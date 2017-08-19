using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class PanIdCommand : AtCommand
    {
        public PanIdCommand() : base("ID")
        {
        }

        public PanIdCommand(ushort id) : this()
        {
            Parameter = id;
        }

        [Ignore]
        public ushort Id => (ushort?) Parameter ?? default(ushort);
    }
}
