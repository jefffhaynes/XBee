using System;
using System.Text;
using System.Threading.Tasks;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee.Tester
{
    class Program
    {
        private static XBeeController _xbee = new XBeeController();

        private static void Main(string[] args)
        {
            //using (var serialConnection = new SerialConnection("COM5", 9600))
            //{
            //    serialConnection.FrameReceived += SerialConnectionOnFrameReceived;
            //    serialConnection.OpenAsync();

            //    var frame = new AtCommandFrameContent("ND") {FrameId = 1};
            //    serialConnection.Send(frame);

            //    Console.ReadKey();

            //    var frame2 = new AtCommandFrameContent("ND") {FrameId = 2};
            //    serialConnection.Send(frame2);


            //    Console.ReadKey();s
            //}

            MainAsync();

            Console.ReadKey();

            _xbee.Dispose();
        }

        private static async void MainAsync()
        {
            _xbee.DataReceived += (sender, eventArgs) => Console.WriteLine("Received {0} bytes", eventArgs.Data.Length);

            //await _xbee.OpenAsync("COM4", 115200);

            await _xbee.OpenAsync("COM5", 9600);

            _xbee.SampleReceived += (sender, args) => Console.WriteLine("Sample recieved: {0}", args);

            //var coordinator = await _xbee.IsCoordinator();

            //var name = await _xbee.GetNodeIdentification();

            //var sleepMode = await _xbee.Local.GetSleepMode();

            //await _xbee.SetNodeIdentifier("COORD 900HP");

            //await _xbee.WriteChanges();

            //name = await _xbee.GetNodeIdentification();

            //var serialNumber = await _xbee.GetSerialNumber();
            Discover();
        }

        private static async void Discover()
        {
            //await _xbee.OpenAsync("COM5", 9600);

            Console.WriteLine("Discovering network...");

            _xbee.NodeDiscovered += async (sender, args) =>
            {
                Console.WriteLine("Discovered '{0}'", args.Name);
                //Console.WriteLine("Sending data to '{0}'", args.Name);
                //await _xbee.TransmitDataAsync(args.Address, Encoding.ASCII.GetBytes("Hello!"));
                //Console.WriteLine("Ack from '{0}'!", args.Name);

                //var changeDetection = await args.Node.GetChangeDetectionChannels();
                //var ee = await args.Node.IsEncryptionEnabled();

                //var id = await args.Node.GetNodeIdentifier();

                await args.Node.Reset();

                Console.WriteLine("reset");

                //await args.Node.ForceSample();
            };

            await _xbee.DiscoverNetwork();

            //await _xbee.ExecuteMultiQueryAsync(new NetworkDiscoveryCommand(), new Action<AtCommandResponseFrame>(
            //    async frame =>
            //    {
            //        Console.WriteLine(frame.Data);
            //        var node = frame.Data as NetworkDiscoveryResponseData;
            //        if (node != null && !node.IsCoordinator)
            //        {
            //            Console.WriteLine("Sending data to {0}", node.Name);
            //            await _xbee.TransmitDataAsync(node.LongAddress, Encoding.ASCII.GetBytes("Hello!"));
            //            Console.WriteLine("Received!");
            //        }
            //    }), TimeSpan.FromSeconds(6));

            Console.WriteLine("Done.");
        }

        //private static void SerialConnectionOnFrameReceived(object sender, FrameReceivedEventArgs e)
        //{
        //    Console.WriteLine(e.FrameContent.GetType());
        //}
    }
}
