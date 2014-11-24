namespace XBee.Frames.AtCommands
{
    public class PrimitiveResponseData<TValue> : AtCommandResponseFrameData where TValue : struct
    {
        public TValue Value { get; set; }
    }
}
