using System;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee.Converters
{
    internal class VersionToSeriesConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            var hardwareVersion = (HardwareVersion) value;
            return DeviceFactory.GetSeries(hardwareVersion);
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
