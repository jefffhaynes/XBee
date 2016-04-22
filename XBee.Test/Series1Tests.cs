using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XBee.Test
{
    [TestClass]
    public class Series1Tests : DeviceTestBase
    {
        [TestMethod]
        public async Task OpenCloseTest()
        {
            await OpenCloseTestBase();
        }

        [TestMethod]
        public async Task OpenCloseCycleTest()
        {
            await OpenCloseCycleTestBase();
        }

        [TestMethod]
        public async Task LocalReadSerialNumberTest()
        {
            await LocalReadSerialNumberTestBase();
        }

        [TestMethod]
        public async Task LocalReadWriteIdTest()
        {
            await LocalReadWriteIdTestBase();
        }

        [TestMethod]
        public async Task LocalReadWriteSleepModeTest()
        {
            await LocalReadWriteSleepModeTestBase();
        }

        [TestMethod]
        public async Task LocalReadWriteSleepOptionsTest()
        {
            await LocalReadWriteSleepOptionsTestBase();
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

        [TestMethod]
        public async Task LocalReadWriteInputOutputConfigurationTest()
        {
            await LocalReadWriteInputOutputConfigurationTestBase();
        }

        [TestMethod]
        public async Task LocalReadEncryptionEnableTest()
        {
            await LocalReadEncryptionEnableTestBase();
        }
    }
}
