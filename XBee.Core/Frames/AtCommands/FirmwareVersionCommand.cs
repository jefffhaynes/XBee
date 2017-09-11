namespace XBee.Frames.AtCommands
{
    internal class FirmwareVersionCommand : AtCommand
    {
        public const string Name = "VR";

        public FirmwareVersionCommand() : base(Name)
        {
        }
    }
}
