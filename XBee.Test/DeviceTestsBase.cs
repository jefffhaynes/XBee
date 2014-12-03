using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XBee.Test
{
    public abstract class DeviceTestsBase
    {
        public static XBeeController Controller { get; private set; }

        public XBeeNode Local { get { return Controller.Local; } }

        protected async static void OpenDevice(int baud)
        {
            Controller = new XBeeController();
            await Controller.OpenAsync("COM4", baud);
        }

        public virtual async Task LocalReadWriteIdTest()
        {
            const string testId = "TEST ID";
            var id = await Local.GetNodeIdentifier();
            await Local.SetNodeIdentifier(testId);
            Assert.AreEqual(testId, await Local.GetNodeIdentifier());
            await Local.SetNodeIdentifier(id);
        }
    }
}
