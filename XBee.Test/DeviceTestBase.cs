using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XBee.Devices;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Test
{
    public abstract class DeviceTestBase
    {
        private static XBeeSeries1 _device;

        private static async Task<XBeeSeries1> GetDevice()
        {
            if (_device == null)
            {
                var controller = await XBeeController.FindAndOpenAsync(SerialPort.GetPortNames(), 9600);
                _device = controller.Local as XBeeSeries1;
            }

            return _device;
        }

        public async Task OpenCloseTestBase()
        {
            var controller = await XBeeController.FindAndOpenAsync(SerialPort.GetPortNames(), 9600);
            controller.Close();
        }

        public async Task OpenCloseCycleTestBase()
        {
            var controller = await XBeeController.FindAndOpenAsync(SerialPort.GetPortNames(), 9600);
            controller.Close();

            for (int i = 0; i < 10; i++)
            {
                await controller.OpenAsync();
                controller.Close();
            }
        }

        public async Task LocalReadSerialNumberTestBase()
        {
            var device = await GetDevice();
            await device.GetSerialNumberAsync();
        }

        public async Task LocalReadWriteIdTestBase()
        {
            var device = await GetDevice();

            const string testId = "TEST ID";
            var id = await device.GetNodeIdentifierAsync();
            await device.SetNodeIdentifierAsync(testId);
            Assert.AreEqual(testId, await device.GetNodeIdentifierAsync());
            await device.SetNodeIdentifierAsync(id);
        }

        public async Task LocalReadWriteSleepModeTestBase()
        {
            var device = await GetDevice();

            var mode = await device.GetSleepModeAsync();
            await device.SetSleepModeAsync(SleepMode.CyclicSleepWithPinWake);
            Assert.AreEqual(SleepMode.CyclicSleepWithPinWake, await device.GetSleepModeAsync());
            await device.SetSleepModeAsync(mode);
        }

        public async Task LocalReadWriteSleepOptionsTestBase()
        {
            var device = await GetDevice();

            var options = await device.GetSleepOptionsAsync();
            await device.SetSleepOptionsAsync(SleepOptions.SampleOnWakeDisable);
            Assert.AreEqual(SleepOptions.SampleOnWakeDisable, await device.GetSleepOptionsAsync());
            await device.SetSleepOptionsAsync(options);
        }

        public async Task LocalReadWriteChangeDetectionTestBase()
        {
            var device = await GetDevice();

            var channels = await device.GetChangeDetectionChannelsAsync();
            channels ^= DigitalSampleChannels.Input4;
            await device.SetChangeDetectionChannelsAsync(channels);

            Assert.AreEqual(channels, await device.GetChangeDetectionChannelsAsync());

            channels ^= DigitalSampleChannels.Input4;

            await device.SetChangeDetectionChannelsAsync(channels);
        }

        public async Task LocalReadWriteSampleRateTestBase()
        {
            var device = await GetDevice();

            var testSampleRate = TimeSpan.FromMilliseconds(ushort.MaxValue);

            var sampleRate = await device.GetSampleRateAsync();
            await device.SetSampleRateAsync(testSampleRate);

            Assert.AreEqual(testSampleRate, await device.GetSampleRateAsync());

            await device.SetSampleRateAsync(sampleRate);
        }

        public async Task LocalReadWriteInputOutputConfigurationTestBase()
        {
            var device = await GetDevice();

            var ioConfig = await device.GetInputOutputConfigurationAsync(InputOutputChannel.Channel5);

            await device.SetInputOutputConfigurationAsync(InputOutputChannel.Channel5, InputOutputConfiguration.AnalogIn);

            Assert.AreEqual(InputOutputConfiguration.AnalogIn, await device.GetInputOutputConfigurationAsync(InputOutputChannel.Channel5));

            await device.SetInputOutputConfigurationAsync(InputOutputChannel.Channel5, ioConfig);
        }

        public async Task LocalReadEncryptionEnableTestBase()
        {
            var device = await GetDevice();
            await device.IsEncryptionEnabledAsync();
        }
    }
}
