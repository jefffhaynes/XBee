
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XBee.Frames;

namespace XBee.Test
{
    [TestClass]
    public class FrameTest
    {
        private readonly FrameSerializer _frameSerializer = new FrameSerializer();

        private void Check(FrameContent frameContent, byte[] expectedValue)
        {
            var frame = new Frame(frameContent);
            var data = _frameSerializer.Serialize(frame);
            Assert.IsTrue(data.SequenceEqual(expectedValue));
        }

        [TestMethod]
        public void AtCommandFrameTest()
        {
            var atCommandFrame = new AtCommandFrameContent("NH") {FrameId = 0x52};

            var expectedValue = new byte[] { 0x7e, 0x00, 0x04, 0x08, 0x52, 0x4e, 0x48, 0x0f };

            Check(atCommandFrame, expectedValue);
        }

        [TestMethod]
        public void AtCommandResponseFrameTest()
        {
            var atResponseCommandFrame = new AtCommandResponseFrame {AtCommand = "BD", FrameId = 0x01};

            var expectedValue = new byte[] { 0x7e, 0x00, 0x05, 0x88, 0x01, 0x42, 0x44, 0x00, 0xf0 };

            Check(atResponseCommandFrame, expectedValue);
        }

        [TestMethod]
        public void TxRequestFrameTest()
        {
            var txRequestFrame = new TxRequestExtFrame(new LongAddress(0x0013A200400A0127),
                new byte[] {0x54, 0x78, 0x44, 0x61, 0x74, 0x61, 0x30, 0x41}) {FrameId = 0x01};

            var expectedValue = new byte[]
            {
                0x7e, 0x00, 0x16, 0x10, 0x01, 0x00, 0x13, 0xA2, 
                0x00, 0x40, 0x0A, 0x01, 0x27, 0xff, 0xfe, 0x00, 
                0x00, 0x54, 0x78, 0x44, 0x61, 0x74, 0x61, 0x30, 
                0x41, 0x13
            };

            Check(txRequestFrame, expectedValue);
        }

        [TestMethod]
        public void TxStatusExtFrameTest()
        {
            var txStatusFrame = new TxStatusExtFrame
            {
                FrameId = 0x47,
                DiscoveryStatus = DiscoveryStatus.RouteDiscovery
            };

            var expectedValue = new byte[] {0x7e, 0x00, 0x07, 0x8b, 0x47, 0xff, 0xfe, 0x00, 0x00, 0x02, 0x2e};

            Check(txStatusFrame, expectedValue);
        }
    }
}
