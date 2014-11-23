using System.Linq;

namespace XBee
{
    public static class Crc
    {
        public static byte Calculate(byte[] data)
        {
            var checksum = data.Aggregate(0, (current, b) => current + b);

            // discard values > 1 byte
            checksum = 0xff & checksum;
            // perform 2s complement
            checksum = 0xff - checksum;

            return (byte)checksum;
        }
    }
}
