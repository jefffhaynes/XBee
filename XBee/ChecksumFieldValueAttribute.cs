using BinarySerialization;

namespace XBee
{
    public class ChecksumFieldValueAttribute : FieldValueAttributeBase
    {
        private int _checksum;

        public ChecksumFieldValueAttribute(string valuePath) : base(valuePath)
        {
        }

        protected override void Reset(BinarySerializationContext context)
        {
            _checksum = 0;
        }

        protected override void Compute(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
                _checksum = _checksum + buffer[i];
        }

        protected override object ComputeFinal()
        {
            // discard values > 1 byte
            var checksum = 0xff & _checksum;

            // perform 2s complement
            checksum = 0xff - checksum;

            return (byte) checksum;
        }
    }
}
