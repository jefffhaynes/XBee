using System;
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
            var openTask = controller.OpenAsync("COM4", 9600);
            openTask.Wait();

            if (controller == null)
            {
                return;
            }
            
            var local = controller.Local as XBeeSeries1;
            

            //var panIdTask = local.GetPanIdAsync();
            //panIdTask.Wait();

            //var panId = panIdTask.Result;

            var pullupTask = local.GetPullUpResistorConfigurationAsync();
            pullupTask.Wait();

            var pullupConfig = pullupTask.Result;

            var rssiCycleTask = local.GetRssiPwmTimeAsync();
            rssiCycleTask.Wait();

            var rssiCycle = rssiCycleTask.Result;

            Console.WriteLine("Found controller.");
            controller.NodeDiscovered += async (sender, eventArgs) =>
            {
                Console.WriteLine($"{eventArgs.Name} discovered.");
                var node = eventArgs.Node as XBeeSeries2Base;

                if (node != null)
                {
                    await node.TransmitDataAsync(new byte[] {0, 0});
                    Console.WriteLine("sent.");
                }
            };
            var discoverTask = controller.DiscoverNetworkAsync();
            discoverTask.Wait();
            Console.WriteLine("Discovery finished.");
            Console.ReadKey();
        }
    }
}
