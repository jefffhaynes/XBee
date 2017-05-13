using Windows.Devices.SerialCommunication;

namespace XBee.Universal
{
    public class XBeeController : XBee.XBeeController
    {
        public XBeeController(SerialDevice serialDevice) : base(new SerialDeviceWrapper(serialDevice))
        {
        }
    }
}
