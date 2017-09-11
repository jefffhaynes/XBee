namespace XBee.Frames.AtCommands
{
    internal class HardwareVersionCommand : AtCommand
    {
        public const string Name = "HV";

        public HardwareVersionCommand() : base(Name)
        {
        }
    }
}
