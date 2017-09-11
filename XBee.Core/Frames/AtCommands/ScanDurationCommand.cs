namespace XBee.Frames.AtCommands
{
    internal class ScanDurationCommand : AtCommand
    {
        public const string Name = "SD";

        public ScanDurationCommand() : base(Name)
        {
        }

        public ScanDurationCommand(byte exponent) : this()
        {
            Parameter = exponent;
        }

        public byte GetExponent()
        {
            return (byte) Parameter;
        }
    }
}
