namespace XBee.Frames.AtCommands
{
    public class InputOutputChangeDetectionResponseData : AtCommandResponseFrameData
    {
        public DigitalSampleChannels Channels { get; set; }
    }
}
