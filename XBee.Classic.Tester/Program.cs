using System;

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

            Console.WriteLine("Found controller.");
            controller.NodeDiscovered += (sender, eventArgs) => Console.WriteLine($"{eventArgs.Name} discovered.");
            var discoverTask = controller.DiscoverNetworkAsync();
            discoverTask.Wait();
            Console.WriteLine("Discovery finished.");
            Console.ReadKey();
        }
    }
}
