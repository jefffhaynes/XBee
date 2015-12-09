using System;
using System.Threading.Tasks;

namespace XBee
{
    internal static class TaskExtensions
    {
        public static async Task Retry(Action action, Type expectedExceptionType, TimeSpan timeout)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception e)
                {
                    if(e.GetType() != expectedExceptionType)
                        throw;
                }
                await Task.Delay(100);
            }
        }
    }
}
