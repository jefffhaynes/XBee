using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class EncryptionEnableCommand : AtCommand
    {
        public EncryptionEnableCommand() : base("EE")
        {
        }

        public EncryptionEnableCommand(bool enabled) : this()
        {
            Enabled = enabled;
        }

        [Ignore]
        public bool? Enabled
        {
            get => Parameter as bool?;
            set => Parameter = value;
        }
    }
}
