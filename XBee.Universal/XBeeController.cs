using Windows.Devices.SerialCommunication;

namespace XBee.Universal
{
    /// <summary>
    /// 
    /// </summary>
    public class XBeeController : Core.XBeeController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialDevice"></param>
        public XBeeController(SerialDevice serialDevice) : base(new SerialDeviceWrapper(serialDevice))
        {
        }
    }
}
