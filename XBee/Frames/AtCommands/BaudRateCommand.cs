namespace XBee.Frames.AtCommands
{
    public class BaudRateCommand : AtCommand
    {
        public BaudRateCommand() : base("BD")
        {
        }

        public BaudRateCommand(BaudRate baudRate) : this()
        {
            Parameter = baudRate;
        }

        public BaudRateCommand(int baudRate) : this()
        {
            Parameter = baudRate;
        }
    }
}
