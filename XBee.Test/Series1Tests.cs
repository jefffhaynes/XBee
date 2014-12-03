using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XBee.Test
{
    [TestClass]
    public class Series1Tests : DeviceTestsBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            OpenDevice(9600);
        }

        [TestMethod]
        public override async Task LocalReadWriteIdTest()
        {
            await base.LocalReadWriteIdTest();
        }
    }
}
