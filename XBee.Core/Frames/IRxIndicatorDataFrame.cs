namespace XBee.Frames
{
    internal interface IRxIndicatorDataFrame : IRxIndicatorFrame
    {
        byte[] Data { get; set; }
    }
}
