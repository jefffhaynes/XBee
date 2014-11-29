using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class EncryptionEnableCommand : AtCommand
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
            get { return Parameter as bool?; }
            set { Parameter = value; }
        }
    }
}
