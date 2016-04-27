using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class EncryptionKeyCommand : AtCommand
    {
        private const int KeyLength = 16;

        public EncryptionKeyCommand() : base("KY")
        {
        }

        public EncryptionKeyCommand(byte[] key) : this()
        {
            if(key == null)
                throw new ArgumentNullException(nameof(key));

            if(key.Length != KeyLength)
                throw new ArgumentException($"Key length must be {KeyLength}", nameof(key));

            Key = key;
        }

        [Ignore]
        public byte[] Key
        {
            get { return Parameter as byte[]; }
            set { Parameter = value; }
        }
    }
}
