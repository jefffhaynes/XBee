using System;
using System.Linq;
using XBee.Devices;

namespace XBee.Classic.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //var controllerTask = XBeeController.FindAndOpenAsync(9600);
            //controllerTask.Wait();
            //var controller = controllerTask.Result;

            var controller = new XBeeController();
            var openTask = controller.OpenAsync("COM4", 115200);
            openTask.Wait();

            if (controller == null)
            {
                return;
            }
            
            var s2 = new XBeeSeries2(controller);
            var s2Task = s2.GetPanIdAsync();
            s2Task.Wait();

            return;

            var local = controller.Local as XBeeSeries1;

            controller.ModemStatusChanged += (sender, eventArgs) =>
            {
                Console.WriteLine($"Modem status: {eventArgs.Status}");
            };

            var panIdTask = local.GetPanIdAsync();
            panIdTask.Wait();

            //var panId = panIdTask.Result;

            //var pullupTask = local.GetPullUpResistorConfigurationAsync();
            //pullupTask.Wait();

            //var pullupConfig = pullupTask.Result;

            //var rssiCycleTask = local.GetRssiPwmTimeAsync();
            //rssiCycleTask.Wait();

            //var rssiCycle = rssiCycleTask.Result;

            Console.WriteLine("Found controller.");
            controller.NodeDiscovered += (sender, eventArgs) =>
            {
                Console.WriteLine($"{eventArgs.Name} discovered.");
                eventArgs.Node.TransmitDataAsync(Enumerable.Repeat((byte) 5, 400).ToArray());
            };
            var discoverTask = controller.DiscoverNetworkAsync();
            discoverTask.Wait();
            Console.WriteLine("Discovery finished.");
            Console.ReadKey();
        }
    }
}
