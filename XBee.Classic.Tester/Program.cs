using System;
using System.Linq;
using System.Threading.Tasks;
using XBee.Devices;
using XBee.Frames.AtCommands;

namespace XBee.Classic.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //var controllerTask = XBeeController.FindAndOpenAsync(9600);
            //controllerTask.Wait();
            //var controller = controllerTask.Result;

            Task.Run(async () =>
            {
                var controller = new XBeeController();
                await controller.OpenAsync("COM3", 115200);

                var s2 = new XBeeSeries2(controller);
                s2.SampleReceived += (sender, eventArgs) => Console.WriteLine("SAMPLE ---------------");
                await s2.SetInputOutputConfigurationAsync(InputOutputChannel.Channel0,
                    InputOutputConfiguration.DigitalIn);
                await s2.SetSampleRateAsync(TimeSpan.FromSeconds(3));
                await s2.ForceSampleAsync();
            });

            Console.ReadKey();
        }
    }
}
