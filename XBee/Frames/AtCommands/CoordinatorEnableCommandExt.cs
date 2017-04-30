using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class CoordinatorEnableCommandExt : AtCommand
    {
        public CoordinatorEnableCommandExt() : base("CE")
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
