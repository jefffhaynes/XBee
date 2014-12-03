using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XBee.Test
{
    [TestClass]
    public class Series1Tests : DeviceTestBase
    {
        [TestMethod]
        public async Task LocalReadWriteIdTest()
        {
            await LocalReadWriteChangeDetectionTestBase();
        }

        [TestMethod]
        public async Task LocalReadWriteChangeDetectionTest()
        {
            await LocalReadWriteChangeDetectionTestBase();
        }

        [TestMethod]
        public async Task LocalReadWriteSampleRateTest()
        {
            await LocalReadWriteSampleRateTestBase();
        }
    }
}
