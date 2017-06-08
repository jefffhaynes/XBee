namespace XBee.Frames.AtCommands
{
    internal class BaudRateCommand : AtCommand
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
            // this command parameter somewhat strangely will take any int size so we're only supposed to send as many bytes as needed
            if (baudRate < ushort.MaxValue)
            {
                Parameter = (ushort) baudRate;
            }
            else
            {
                Parameter = baudRate;
            }
        }
    }
}
