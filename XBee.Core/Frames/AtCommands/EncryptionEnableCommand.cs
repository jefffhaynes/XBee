using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class EncryptionEnableCommand : AtCommand
    {
        public const string Name = "EE";

        public EncryptionEnableCommand() : base(Name)
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
