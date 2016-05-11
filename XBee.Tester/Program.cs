using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Devices;
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

            Console.SetBufferSize(80, 10000);

            MainAsync();

            Console.ReadKey();

            _xbee.Dispose();
        }

        private static async void MainAsync()
        {
            //_xbee = new XBeeController();

            ////await _xbee.OpenAsync("COM5", 9600);

            ////_xbee = await XBeeController.FindAndOpen(SerialPort.GetPortNames(), 115200);



            //await _xbee.OpenAsync("COM4", 115200);

            _xbee = await XBeeController.FindAndOpenAsync(SerialPort.GetPortNames(), 9600);

            _xbee.FrameMemberSerializing += XbeeOnFrameMemberSerializing;
            _xbee.FrameMemberSerialized += XbeeOnFrameMemberSerialized;
            _xbee.FrameMemberDeserializing += XbeeOnFrameMemberDeserializing;
            _xbee.FrameMemberDeserialized += XbeeOnFrameMemberDeserialized;

            //_xbee.DataReceived += (sender, eventArgs) => Console.WriteLine("Received {0} bytes", eventArgs.Data.Length);

            //Console.WriteLine("Running {0}", _xbee.HardwareVersion);

            _xbee.SampleReceived += (sender, args) => Console.WriteLine($"Sample: {args.Address}: {args.DigitalChannels}-{args.DigitalSampleState} + {string.Join(", ",args.AnalogSamples)}");

            //var coordinator = await _xbee.IsCoordinator();

            //var name = await _xbee.GetNodeIdentification();

            //var sleepMode = await _xbee.Local.GetSleepMode();

            //await _xbee.SetNodeIdentifier("COORD 900HP");

            //await _xbee.WriteChanges();

            //name = await _xbee.GetNodeIdentification();

            //var serialNumber = await _xbee.GetSerialNumber();
            Discover();


        }


        private static void XbeeOnFrameMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            Console.WriteLine("S-Start: {0}", e.MemberName);
        }

        private static void XbeeOnFrameMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            var value = e.Value ?? "null";
            Console.WriteLine("S-End: {0} ({1})", e.MemberName, value);
        }

        private static void XbeeOnFrameMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            Console.WriteLine("D-Start: {0}", e.MemberName);
        }

        private static void XbeeOnFrameMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            var value = e.Value ?? "null";
            Console.WriteLine("D-End: {0} ({1})", e.MemberName, value);
        }

        private static async Task Toggle(XBeeSeries2 node, int iteration)
        {
            Console.WriteLine(iteration);
            await node.SetInputOutputConfigurationAsync(InputOutputChannel.Channel4, InputOutputConfiguration.DigitalLow);
            await node.SetInputOutputConfigurationAsync(InputOutputChannel.Channel4, InputOutputConfiguration.Disabled);
        }

        private static async void Discover()
        {
            //await _xbee.OpenAsync("COM5", 9600);

            Console.WriteLine("Discovering network...");

            _xbee.NodeDiscovered += async (sender, args) =>
            {
                Console.WriteLine("---------------- Discovered '{0}'", args.Name);
                //Console.WriteLine("Sending data to '{0}'", args.Name);
                //await args.Node.TransmitDataAsync(Encoding.ASCII.GetBytes("Hello!"));

                //var node = args.Node as XBeeSeries2;

                //var receivedData =
                //    node.GetReceivedData().Subscribe(data => Console.WriteLine("recieved {0} bytes", data.Length));

                //var stopwatch = new Stopwatch();
                //stopwatch.Start();

                //var range = Enumerable.Range(0, 10);
                //await Task.WhenAll(range.Select(i => Toggle(node, i)));

                //for (int i = 0; i < 100; i++)
                //{
                //    await node.SetInputOutputConfiguration(InputOutputChannel.Channel4, InputOutputConfiguration.DigitalLow);
                //    await node.SetInputOutputConfiguration(InputOutputChannel.Channel4, InputOutputConfiguration.Disabled);
                //}

                //Console.WriteLine(TimeSpan.FromMilliseconds((double)stopwatch.ElapsedMilliseconds/100));

                //var ai = await node.GetAssociation();

                //Console.WriteLine("Ack from '{0}'!", args.Name);

                //var changeDetection = await args.Node.GetChangeDetectionChannels();
                //var ee = await args.Node.IsEncryptionEnabled();

                // await args.Node.SetNodeIdentifier("BOB");

                //for (int i = 0; i < 1; i++)
                //{
                //    var id = await args.Node.GetNodeIdentifier();
                //    Console.WriteLine(id);
                //}

                //if (args.Name == "ED1")
                //    return;

                //await Task.Delay(1000);
                //await args.Node.SetNodeIdentifier("BOB");

                //await args.Node.Reset();

                //Console.WriteLine("reset");

                //await args.Node.SetInputOutputConfiguration(InputOutputChannel.Channel2, InputOutputConfiguration.DigitalIn);
                //await args.Node.SetInputOutputConfiguration(InputOutputChannel.Channel3, InputOutputConfiguration.AnalogIn);

                //await args.Node.SetChangeDetectionChannels(DigitalSampleChannels.Input2);

                //await args.Node.SetSampleRate(TimeSpan.FromSeconds(5));


                //var address = await args.Node.GetDestinationAddress();
                //await args.Node.SetDestinationAddress(new ShortAddress(0));
                //await args.Node.WriteChanges();

                //var samples = args.Node.GetSamples();

                //await samples.ForEachAsync(sample => Console.WriteLine("{0} ({1})", sample.ToString(), args.Name));

                //await args.Node.ForceSample();
            };

            //await _xbee.DiscoverNetwork();

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

        static void Node_SampleReceived(object sender, SampleReceivedEventArgs e)
        {
            Console.WriteLine("sample!");
        }

        //private static void SerialConnectionOnFrameReceived(object sender, FrameReceivedEventArgs e)
        //{
        //    Console.WriteLine(e.FrameContent.GetType());
        //}
    }
}
