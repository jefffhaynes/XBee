using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBee
{
    public static class TaskExtensions
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
