using Windows.Devices.SerialCommunication;

namespace XBee.Universal
{
    public class XBeeController : Core.XBeeController
    {
        public XBeeController(SerialDevice serialDevice) : base(new SerialDeviceWrapper(serialDevice))
        {
        }
    }
}
