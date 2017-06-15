using System;
using XBee.Devices;

namespace XBee.Classic.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var controllerTask = XBeeController.FindAndOpenAsync(9600);
            controllerTask.Wait();
            var controller = controllerTask.Result;

            if (controller == null)
            {
                return;
            }

            var local = controller.Local as XBeeSeries1;
            var panIdTask = local.GetPanIdAsync();
            panIdTask.Wait();

            var panId = panIdTask.Result;

            var pullupTask = local.GetPullUpResistorConfigurationAsync();
            pullupTask.Wait();

            var pullupConfig = pullupTask.Result;

            var rssiCycleTask = local.GetRssiPwmTimeAsync();
            rssiCycleTask.Wait();

            var rssiCycle = rssiCycleTask.Result;

            Console.WriteLine("Found controller.");
            controller.NodeDiscovered += (sender, eventArgs) => Console.WriteLine($"{eventArgs.Name} discovered.");
            var discoverTask = controller.DiscoverNetworkAsync();
            discoverTask.Wait();
            Console.WriteLine("Discovery finished.");
            Console.ReadKey();
        }
    }
}
