XBee
====

.NET library for [XBee wireless controllers](http://www.digi.com/xbee/) available as a [nuget package](https://www.nuget.org/packages/XBee/).

 * Support for Series1, Series 2, and 900HP
 * Simple async/await command and query model
 * [.NET Rx](https://rx.codeplex.com/)  support for async receive and sampling.

###Features###

 * Local and remote device discovery
 * Local and remote device configuration
 * Pin configuration
 * Pin control
 * Pin monitoring
 * Digital and analog sample monitoring via events or Rx
 * Data transmit
 * Data receive via events or Rx

###Quick Start###

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

If you don't know a priori what port the XBee will be attached to you can also scan for it:

```c#
var controller = await XBeeController.FindAndOpen(SerialPort.GetPortNames(), 9600);

if(controller != null)
   // ...
```

###Nodes###

Note that the XBeeController class simply represents the local serial attached XBee API.  This would typically be a coordinator or coordinator-like device but could be any device you want to control via a serial port.

While the controller represents the API, if we want to control the node itself we need to access the local node property.

```c#
var localNode = controller.Local;
var serialNumber = await localNode.GetSerialNumber();
// etc
```

This allows us to treat the local node the same way we treat remote nodes.

```c#
var remoteNode = await controller.GetRemoteAsync(address);
var serialNumber = await remoteNode.GetSerialNumber();
```

The address for the remote node can be determined in a number of ways.  Either connect the remote node to a serial port and use one of the X-CTU utilities (or the above code) or use network discovery.

Now that we have some nodes, let's do something with them...

###Command and Events###

XBees are based on a sort of command-event model where the coordinator is either telling the XBee to do something or the XBee is telling the coordinator that something happened.  Everything else in the API is simply configuration, required to make those two things happen.

####Commands####

The first type of command is essentially GPIO (General Purpose Input/Output), although ignoring the input part for the moment.  We can set pins high or low on a node by "configuring" a pin to a fixed state.

```c#
await node.SetInputOutputConfiguration(InputOutputChannel.Channel4, InputOutputConfiguration.DigitalHigh);
```

This will force pin DIO4 high.  Which physical pin this translates to depends on the model XBee.

The second type of command involves simply sending serial data to a node.  In the simplest case this can act as a transparent passthrough as most XBees will simply pass the serial data to their local UART.  However, in the case of programmable XBees it is possible to intercept the serial data and thereby define more expressive custom protocols.

```c#
await node.TransmitDataAsync(Encoding.UTF8.GetBytes("Hello!"));
```

###Events###

Somewhat confusingly, XBees have two different mechanisms for asychronously sending data to the coordinator.  The first is sampling and the second is simply data.  Samples coorespond to our pin example from above and represent the input part of GPIO.

As such, we can configure a pin to take and send a sample to the coordinator.  

```c#
// first we'll subscribe to the node
node.SampleReceived += (o, eventArgs) => Console.WriteLine(eventArgs.DigitalSampleState);

// then configure a pin for digital sampling
await node.SetInputOutputConfiguration(InputOutputChannel.Channel5, InputOutputConfiguration.DigitalIn);
```

At this point the node is set to send samples from pin DIO5.  There are three ways to trigger a sample: forced, periodic, or change detect.

```c#
await node.ForceSample(); // force
```
```c#
await node.SetSampleRate(TimeSpan.FromSeconds(5)); // periodic
```
```c#
await node.SetChangeDetectionChannels(DigitalSampleChannels.Input5); // change detect
```

The second mechanism for asynchronous remote data transmit is simply the receive side of the transparent (or intercepted) serial channel.

```c#
node.DataReceived += (o, eventArgs) => Console.WriteLine("Received {0} bytes", eventArgs.Data.Length);
```

Again, this data would either be supplied by the external UART on the remote node or by the microcontroller on the remote node.