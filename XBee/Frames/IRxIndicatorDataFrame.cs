namespace XBee.Frames
{
    interface IRxIndicatorDataFrame : IRxIndicatorFrame
    {
        byte[] Data { get; set; }
    }
}
