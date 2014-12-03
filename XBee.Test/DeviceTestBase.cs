using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XBee.Devices;
using XBee.Frames;

namespace XBee.Test
{
    public abstract class DeviceTestBase
    {
        private static XBeeSeries1 _device;

        private static async Task<XBeeSeries1> GetDevice()
        {
            if (_device == null)
            {
                var controller = new XBeeController();
                await controller.OpenAsync("COM5", 9600);
                _device = controller.Local as XBeeSeries1;
            }

            return _device;
        }

        public async Task LocalReadWriteIdTestBase()
        {
            var device = await GetDevice();

            const string testId = "TEST ID";
            var id = await device.GetNodeIdentifier();
            await device.SetNodeIdentifier(testId);
            Assert.AreEqual(testId, await device.GetNodeIdentifier());
            await device.SetNodeIdentifier(id);
        }

        public async Task LocalReadWriteChangeDetectionTestBase()
        {
            var device = await GetDevice();

            var channels = await device.GetChangeDetectionChannels();
            channels ^= DigitalSampleChannels.Input4;
            await device.SetChangeDetectionChannels(channels);

            Assert.AreEqual(channels, await device.GetChangeDetectionChannels());

            channels ^= DigitalSampleChannels.Input4;

            await device.SetChangeDetectionChannels(channels);
        }

        public async Task LocalReadWriteSampleRateTestBase()
        {
            var device = await GetDevice();

            var testSampleRate = TimeSpan.FromMilliseconds(ushort.MaxValue);

            var sampleRate = await device.GetSampleRate();
            await device.SetSampleRate(testSampleRate);

            Assert.AreEqual(testSampleRate, await device.GetSampleRate());

            await device.SetSampleRate(sampleRate);
        }
    }
}
