using System;
using BinarySerialization;

namespace XBee.Converters
{
    public class BitwiseAndConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext ctx)
        {
            if(parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Must specify a mask.");

            var mask = System.Convert.ToInt32(parameter);

            var operand = System.Convert.ToInt32(value);
            
            return operand & mask;
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext ctx)
        {
            throw new NotSupportedException();
        }
    }
}
