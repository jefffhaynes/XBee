XBee
====

.NET library for [XBee wireless controllers](http://www.digi.com/xbee/) available as a [nuget package](https://www.nuget.org/packages/XBee/).

 * Support for Series1, Series 2, and 900HP
 * Simple async/await command and query model
 * [.NET Rx](https://rx.codeplex.com/)  support for received data and sampling.


Simple example with a coordinator on COM3 and an arbitrary number of end devices that we're going to configure and monitor for sampling.
```C#
var controller = new XBeeController();

controller.NodeDiscovered += (sender, args) => 
{
    Console.WriteLine("Discovered {0}", args.Name);
    
    await args.Node.SetInputOutputConfiguration(InputOutputChannel.Channel2, InputOutputConfiguration.DigitalIn);
    await args.Node.SetInputOutputConfiguration(InputOutputChannel.Channel3, InputOutputConfiguration.AnalogIn);
    
    await args.Node.SetChangeDetectionChannels(DigitalSampleChannels.Input2);
    
    await args.Node.SetSampleRate(TimeSpan.FromSeconds(5));
    
    args.Node.SampleReceived += (node, sample) => Console.WriteLine("Sample recieved: {0}", sample);
}

await controller.OpenAsync("COM3", 9600);
await controller.DiscoverNetwork();

Console.ReadKey();

```
