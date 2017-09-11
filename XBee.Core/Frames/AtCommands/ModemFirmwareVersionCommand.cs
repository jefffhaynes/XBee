namespace XBee.Frames.AtCommands
{
    internal class ModemFirmwareVersionCommand : AtCommand
    {
        public const string Name = "MV";

        public ModemFirmwareVersionCommand() : base(Name)
        {
        }
    }
}
