using BinarySerialization;

namespace XBee.Frames
{
    public abstract class AtCommand
    {
        private const int AtCommandFieldLength = 2;

        protected AtCommand(string command)
        {
            Command = command;
        }

        [FieldLength(AtCommandFieldLength)]
        public string Command { get; set; }

        public object Parameter { get; set; }
    }
}
