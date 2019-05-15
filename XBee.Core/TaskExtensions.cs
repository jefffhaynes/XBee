using System;
using System.Linq;
using System.Threading.Tasks;

namespace XBee
{
    internal static class TaskExtensions
    {
        public static async Task<T> Retry<T>(Func<Task<T>> func, TimeSpan timeout, params Type[] allowedExceptionTypes)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    return await func().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (allowedExceptionTypes.All(exception => e.GetType() != exception))
                    {
                        throw;
                    }
                }
                await Task.Delay(100).ConfigureAwait(false);
            }

            throw new TimeoutException();
        }
    }
}