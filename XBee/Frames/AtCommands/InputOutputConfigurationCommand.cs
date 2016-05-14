using System;
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputConfigurationCommand : AtCommand
    {
        public InputOutputConfigurationCommand()
        {
        }

        public InputOutputConfigurationCommand(InputOutputChannel channel) :
         base((int)channel > 9 ? $"P{(int)channel - 10}" : $"D{(int)channel}")
        {
        }

        public InputOutputConfigurationCommand(InputOutputChannel channel, 
            InputOutputConfiguration configuration) : this(channel)
        {
            Configuration = configuration;
        }

        [Ignore]
        public InputOutputConfiguration? Configuration
        {
            get { return Parameter as InputOutputConfiguration?; }
            set { Parameter = Convert.ToByte(value); }
        }
    }
}
