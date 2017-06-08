using BinarySerialization;

namespace XBee.Frames
{
    internal abstract class AtCommand
    {
        private const int AtCommandFieldLength = 2;

        protected AtCommand()
        {
        }

        protected AtCommand(string command)
        {
            Command = command;
        }

        [FieldOrder(0)]
        [FieldLength(AtCommandFieldLength)]
        public string Command { get; set; }

        [FieldOrder(1)]
        public object Parameter { get; set; }
    }
}
