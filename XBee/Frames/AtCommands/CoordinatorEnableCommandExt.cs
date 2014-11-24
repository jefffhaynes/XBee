using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class CoordinatorEnableCommandExt : AtCommandFrameContent
    {
        public CoordinatorEnableCommandExt() : base("CE")
        {
        }

        public CoordinatorEnableCommandExt(bool enable)
            : this()
        {
            EnableState = enable ? CoordinatorEnableStateExt.NonRoutingCoordinator : CoordinatorEnableStateExt.StandardRouter;
        }

        [Ignore]
        public CoordinatorEnableStateExt? EnableState
        {
            get { return Parameter as CoordinatorEnableStateExt?; }
            set { Parameter = value; }
        }
    }
}
