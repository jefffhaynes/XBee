using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class EncryptionKeyCommand : AtCommandFrame
    {
        private const int KeyLength = 16;

        public EncryptionKeyCommand() : base("KY")
        {
        }

        public EncryptionKeyCommand(byte[] key) : this()
        {
            if(key == null)
                throw new ArgumentNullException("key");

            if(key.Length != KeyLength)
                throw new ArgumentException(string.Format("Key length must be {0}", KeyLength), "key");

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
