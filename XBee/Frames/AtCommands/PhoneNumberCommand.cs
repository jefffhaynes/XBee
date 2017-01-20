using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class PhoneNumberCommand : AtCommand
    {
        public PhoneNumberCommand() : base("PH")
        {
        }

        [Ignore]
        public string PhoneNumber
        {
            get { return Parameter as string; }
            set { Parameter = value; }
        }
    }
}
