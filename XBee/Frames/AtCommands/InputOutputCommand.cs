using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputCommand : AtCommandFrameContent
    {
        public InputOutputCommand(InputOutputChannel channel) :
            base(string.Format("D{0}", channel))
        {
        }

        public InputOutputCommand(InputOutputChannel channel, InputOutputState state) : this(channel)
        {
            State = state;
        }

        [Ignore]
        public InputOutputState? State
        {
            get { return Parameter as InputOutputState?; }
            set { Parameter = value; }
        }
    }
}
