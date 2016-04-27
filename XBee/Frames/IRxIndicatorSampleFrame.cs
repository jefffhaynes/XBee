namespace XBee.Frames
{
    public interface IRxIndicatorSampleFrame : IRxIndicatorFrame
    {
        Sample GetSample();
    }
}
