using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class PanIdCommand : AtCommand
    {
        public const string Name = "ID";

        public PanIdCommand() : base(Name)
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
