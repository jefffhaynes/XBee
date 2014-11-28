using System;
using System.Collections.Generic;
using System.Linq;

namespace XBee
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetFlagValues<T>(this T enumValue) where T : struct
        {
            if (!enumValue.GetType().IsEnum)
                throw new ArgumentException("Must be an enum", "enumValue");

            return Enum.GetValues(typeof (T))
                .Cast<object>()
                .Where(o => ((int) o & (int) (object) enumValue) != 0)
                .Cast<T>();
        }
    }
}
