using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class CoordinatorEnableCommand : AtCommand
    {
        public CoordinatorEnableCommand() : base("CE")
        {
        }

        public CoordinatorEnableCommand(bool enable) : this()
        {
            EnableState = enable ? CoordinatorEnableState.Coordinator : CoordinatorEnableState.EndDevice;
        }

        [Ignore]
        public CoordinatorEnableState? EnableState
        {
            get => Parameter as CoordinatorEnableState?;
            set => Parameter = value;
        }
    }
}
