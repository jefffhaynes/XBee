using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using XBee.Core;

namespace XBee.Universal
{
    /// <summary>
    /// 
    /// </summary>
    public class XBeeController : XBeeControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialDevice"></param>
        public XBeeController(SerialDevice serialDevice) : base(new SerialDeviceWrapper(serialDevice))
        {
        }

        /// <summary>
        /// Find all connected XBee controllers at the given baud rate.
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public static async Task<List<XBeeController>> FindControllersAsync(uint baudRate)
        {
            var controllers = new List<XBeeController>();

            var devices = await DeviceInformation.FindAllAsync(Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector());

            foreach (var device in devices)
            {
                var serialDevice = await TryGetSerialDeviceFromIdAsync(device.Id).ConfigureAwait(false);

                if (serialDevice != null)
                {
                    serialDevice.BaudRate = baudRate;
                    serialDevice.DataBits = 8;

                    var controller = await TryGetControllerAsync(serialDevice).ConfigureAwait(false);

                    if (controller != null)
                    {
                        controllers.Add(controller);
                    }
                    else
                    {
                        serialDevice.Dispose();
                    }
                }
            }

            return controllers;
        }

        private static async Task<SerialDevice> TryGetSerialDeviceFromIdAsync(string id)
        {
            try
            {
                return await Windows.Devices.SerialCommunication.SerialDevice.FromIdAsync(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        private static async Task<XBeeController> TryGetControllerAsync(SerialDevice device)
        {
            var controller = new XBeeController(device);

            try
            {
                await controller.GetHardwareVersionAsync().ConfigureAwait(false);
                return controller;
            }
            catch (TimeoutException)
            {
            }

            return null;
        }
    }
}
