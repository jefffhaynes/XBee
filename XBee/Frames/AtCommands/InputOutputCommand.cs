using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputCommand : AtCommandFrame
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
