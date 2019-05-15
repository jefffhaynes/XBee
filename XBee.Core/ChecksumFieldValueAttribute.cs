using BinarySerialization;

namespace XBee
{
    internal class ChecksumFieldValueAttribute : FieldValueAttributeBase
    {
        public ChecksumFieldValueAttribute(string valuePath) : base(valuePath)
        {
        }

        protected override object GetInitialState(BinarySerializationContext context)
        {
            return 0;
        }

        protected override object GetUpdatedState(object state, byte[] buffer, int offset, int count)
        {
            var checksum = (int)state;

            for (var i = offset; i < count; i++)
            {
                checksum = checksum + buffer[i];
            }

            return checksum;
        }

        protected override object GetFinalValue(object state)
        {
            // discard values > 1 byte
            var checksum = 0xff & (int)state;

            // perform 2s complement
            checksum = 0xff - checksum;

            return (byte) checksum;
        }
    }
}