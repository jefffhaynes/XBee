using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Devices;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBeeTester
{
    class Program

    {

        private static XBee.XBeeController _xbee = new XBee.XBeeController();       // modifié GP

        static void Main(string[] args)
        {

            Console.SetBufferSize(80, 10000);

            MainAsync();

            if (_xbee == null)
                return;

            Console.ReadKey();

            _xbee.Dispose();

        }

        private static async void MainAsync()
        {

            _xbee = await XBee.XBeeController.FindAndOpenAsync(SerialPort.GetPortNames(), 9600);

            if (_xbee == null)
            {
                Console.Write("Pas de module XBee local trouvé");
                Console.ReadKey();
                return;

            }

            Console.WriteLine("Controller:");

            string NodeID = await _xbee.Local.GetNodeIdentifierAsync();

            if (NodeID == null)
                return;

            Console.WriteLine("\tN:{0}", NodeID);

            XBee.LongAddress SerialN = await _xbee.Local.GetSerialNumberAsync();

            Console.WriteLine("\tSHSL:{0}", SerialN);

            //var Baud = await _xbee.Local.GetBaudRateAsync();

            //Console.WriteLine("\tBD:{0}", Baud);

            _xbee.FrameMemberSerializing += XbeeOnFrameMemberSerializing;
            _xbee.FrameMemberSerialized += XbeeOnFrameMemberSerialized;
            _xbee.FrameMemberDeserializing += XbeeOnFrameMemberDeserializing;
            _xbee.FrameMemberDeserialized += XbeeOnFrameMemberDeserialized;

            _xbee.SampleReceived += (sender, args) => Console.WriteLine($"Sample: {args.Address}: {args.DigitalChannels}-{args.DigitalSampleState} + {string.Join(", ", args.AnalogSamples)}");

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

        private static void Discover()
        {

            Console.WriteLine("Discovering network...");

            _xbee.NodeDiscovered += async (sender, args) =>
            {
                Console.WriteLine("---------------- Discovered '{0}'", args.Name);

            };

            Console.WriteLine("Done.");
        }

        static void Node_SampleReceived(object sender, XBee.SampleReceivedEventArgs e)             // modifié GP
        {
            Console.WriteLine("sample!");
        }
    }

}