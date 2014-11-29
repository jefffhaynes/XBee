using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class InputOutputConfigurationCommandExt : AtCommand
    {
        public InputOutputConfigurationCommandExt(InputOutputChannel channel) :
            base(string.Format("D{0}", (int)channel))
        {
        }

        public InputOutputConfigurationCommandExt(InputOutputChannel channel, 
            InputOutputConfiguration configuration) : this(channel)
        {
            Configuration = configuration;
        }

        [Ignore]
        public InputOutputConfiguration? Configuration
        {
            get { return Parameter as InputOutputConfiguration?; }
            set { Parameter = value; }
        }
    }
}
