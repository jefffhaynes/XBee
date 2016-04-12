using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Test
{
    [TestClass]
    public class FrameTests
    {
        private readonly FrameSerializer _frameSerializer = new FrameSerializer();

        private void Check(FrameContent frameContent, byte[] expectedValue)
        {
            var frame = new Frame(frameContent);
            var actualValue = _frameSerializer.Serialize(frame);

            Assert.AreEqual(expectedValue.Length, actualValue.Length, "Actual data length does not match expected length.");

            for (int i = 0; i < expectedValue.Length; i++)
            {
                var expected = expectedValue[i];
                var actual = actualValue[i];

                Assert.AreEqual(expected, actual, $"Value at position {i} does not match expected value.");
            }
        }

        [TestMethod]
        public void AtCommandFrameTest()
        {
            var atCommandFrame = new AtCommandFrameContent(new ForceSampleCommand()) { FrameId = 0x52 };

            var expectedValue = new byte[] { 0x7e, 0x00, 0x04, 0x08, 0x52, 0x49, 0x53, 0x09 };

            Check(atCommandFrame, expectedValue);
        }

        [TestMethod]
        public void AtCommandResponseFrameTest()
        {
            var atResponseCommandFrame = new AtCommandResponseFrame
            {
                FrameId = 0x01,
                Content = new AtCommandResponseFrameContent {AtCommand = "BD"}
            };

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

            var expectedValue = new byte[] {0x7e, 0x00, 0x07, 0x8b, 0x47, 0xff, 0xff, 0x00, 0x00, 0x02, 0x2d};

            Check(txStatusFrame, expectedValue);
        }

        [TestMethod]
        public void AtCommand_CoordinatorEnable_FrameTest()
        {
            var atCommandFrame = new AtCommandFrameContent(new CoordinatorEnableCommand()) { FrameId = 0x33 };

            var expectedValue = new byte[] { 0x7e, 0x00, 0x04, 0x08, 0x33, 0x43, 0x45, 0x3c };

            Check(atCommandFrame, expectedValue);
        }


        [TestMethod]
        public void AtCommand_CoordinatorEnableWithParam_FrameTest()
        {
            var atCommandFrame = new AtCommandFrameContent(new CoordinatorEnableCommand(true)) { FrameId = 0x33 };

            var expectedValue = new byte[] { 0x7e, 0x00, 0x05, 0x08, 0x33, 0x43, 0x45, 0x1, 0x3b };

            Check(atCommandFrame, expectedValue);
        }

        [TestMethod]
        public void AtCommand_SleepMode_FrameTest()
        {
            var data = new byte[] { 0x7E, 0x00, 0x06, 0x88, 0x01, 0x53, 0x4D, 0x00, 0x00, 0xD6 };

            _frameSerializer.Deserialize(new MemoryStream(data));
        }

        //[TestMethod]
        //public void RxIndicatorSampleFrameTest()
        //{
        //    var data = new byte[]
        //    {
        //        0x7e, 0x00, 0x16, 0x82,
        //        0x76, 0x54, 0x32, 0x10, 
        //        0x76, 0x54, 0x32, 0x10, 
        //        0x1b, 0x00, 0x01, 0x0e,
        //        0x58, 0x00, 0x18, 0x00, 
        //        0x46, 0x01, 0x54, 0x02, 
        //        0x0a, 0x10
        //    };

        //    var frame = _frameSerializer.Deserialize(new MemoryStream(data));

        //    var content = frame.Payload.Content as RxIndicatorSampleFrame;

        //    Assert.IsNotNull(content);
        //    Assert.AreEqual(content.AnalogSamples.Count, 3);
        //    Assert.AreEqual(content.AnalogSamples[0], 0x46);
        //    Assert.AreEqual(content.AnalogSamples[1], 0x154);
        //    Assert.AreEqual(content.AnalogSamples[2], 0x20a);
        //}
    }
}
