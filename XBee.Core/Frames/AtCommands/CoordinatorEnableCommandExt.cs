using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class CoordinatorEnableCommandExt : AtCommand
    {
        public CoordinatorEnableCommandExt() : base(CoordinatorEnableCommand.Name)
        {
        }

        public CoordinatorEnableCommandExt(NodeMessagingOptions options)
            : this()
        {
            Options = options;
        }

        [Ignore]
        public NodeMessagingOptions? Options
        {
            get => Parameter as NodeMessagingOptions?;
            set => Parameter = value;
        }
    }
}
