using System;
using System.Linq;
using System.Threading.Tasks;

namespace XBee
{
    internal static class TaskExtensions
    {
        public static async Task Retry(Action action, TimeSpan timeout, Type allowedExceptionType)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception e)
                {
                    if(e.GetType() != allowedExceptionType)
                        throw;
                }
                await Task.Delay(100);
            }

            throw new TimeoutException();
        }

        public static async Task<T> Retry<T>(Func<Task<T>> func, TimeSpan timeout, params Type[] allowedExceptionTypes)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    return await func();
                }
                catch (Exception e)
                {
                    if(allowedExceptionTypes.All(exception => e.GetType() != exception))
                        throw;
                }
                await Task.Delay(100);
            }

            throw new TimeoutException();
        }
    }
}
