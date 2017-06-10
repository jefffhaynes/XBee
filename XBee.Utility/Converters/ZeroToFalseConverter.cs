using System;
using Windows.UI.Xaml.Data;

namespace XBee.Utility.Converters
{
    public class ZeroToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var count = System.Convert.ToInt32(value);
            return count != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
