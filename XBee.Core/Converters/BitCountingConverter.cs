using System;
using BinarySerialization;

namespace XBee.Converters
{
    public class BitCountingConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext ctx)
        {
            var operand = System.Convert.ToInt32(value);

            if (parameter != null)
            {
                var mask = System.Convert.ToInt32(parameter);
                operand &= mask;
            }

            int count;
            for (count = 0; operand > 0; operand >>= 1)
                count += operand & 1;

            return count;
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext ctx)
        {
            throw new NotSupportedException();
        }
    }
}
