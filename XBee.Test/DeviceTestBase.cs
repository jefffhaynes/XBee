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
                var controller = await XBeeController.FindAndOpen(SerialPort.GetPortNames(), 9600);
                _device = controller.Local as XBeeSeries1;
            }

            return _device;
        }

        public async Task OpenCloseTestBase()
        {
            var controller = await XBeeController.FindAndOpen(SerialPort.GetPortNames(), 9600);
            controller.Close();
        }

        public async Task LocalReadSerialNumberTestBase()
        {
            var device = await GetDevice();
            await device.GetSerialNumber();
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

        public async Task LocalReadWriteSleepModeTestBase()
        {
            var device = await GetDevice();

            var mode = await device.GetSleepMode();
            await device.SetSleepMode(SleepMode.CyclicSleepWithPinWake);
            Assert.AreEqual(SleepMode.CyclicSleepWithPinWake, await device.GetSleepMode());
            await device.SetSleepMode(mode);
        }

        public async Task LocalReadWriteSleepOptionsTestBase()
        {
            var device = await GetDevice();

            var options = await device.GetSleepOptions();
            await device.SetSleepOptions(SleepOptions.SampleOnWakeDisable);
            Assert.AreEqual(SleepOptions.SampleOnWakeDisable, await device.GetSleepOptions());
            await device.SetSleepOptions(options);
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

        public async Task LocalReadWriteInputOutputConfigurationTestBase()
        {
            var device = await GetDevice();

            var ioConfig = await device.GetInputOutputConfiguration(InputOutputChannel.Channel5);

            await device.SetInputOutputConfiguration(InputOutputChannel.Channel5, InputOutputConfiguration.AnalogIn);

            Assert.AreEqual(InputOutputConfiguration.AnalogIn, await device.GetInputOutputConfiguration(InputOutputChannel.Channel5));

            await device.SetInputOutputConfiguration(InputOutputChannel.Channel5, ioConfig);
        }

        public async Task LocalReadEncryptionEnableTestBase()
        {
            var device = await GetDevice();
            await device.IsEncryptionEnabled();
        }
    }
}
