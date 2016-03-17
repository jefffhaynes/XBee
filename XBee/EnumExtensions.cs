using System;
using System.Collections.Generic;
using System.Linq;

namespace XBee
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetFlagValues<T>(this T enumValue) where T : struct
        {
#if !WINDOWS_UWP
            if (!enumValue.GetType().IsEnum)
                throw new ArgumentException("Must be an enum", nameof(enumValue));
#endif

            return Enum.GetValues(typeof (T))
                .Cast<object>()
                .Where(o => (Convert.ToInt32(o) & Convert.ToInt32(enumValue)) != 0)
                .Cast<T>();
        }
    }
}
