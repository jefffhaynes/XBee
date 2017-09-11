using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class EncryptionKeyCommand : AtCommand
    {
        public const string Name = "KY";

        private const int KeyLength = 16;

        public EncryptionKeyCommand() : base(Name)
        {
        }

        public EncryptionKeyCommand(byte[] key) : this()
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length != KeyLength)
            {
                throw new ArgumentException($"Key length must be {KeyLength}", nameof(key));
            }

            Key = key;
        }

        [Ignore]
        public byte[] Key
        {
            get => Parameter as byte[];
            set => Parameter = value;
        }
    }
}