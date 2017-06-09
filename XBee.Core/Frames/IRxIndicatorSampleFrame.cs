namespace XBee.Frames
{
    internal interface IRxIndicatorSampleFrame : IRxIndicatorFrame
    {
        Sample GetSample();
    }
}
