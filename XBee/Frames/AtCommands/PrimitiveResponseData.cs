namespace XBee.Frames.AtCommands
{
    public class PrimitiveResponseData<TValue> : AtCommandResponseFrameData
    {
        public TValue Value { get; set; }
    }
}
