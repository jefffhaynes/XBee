namespace XBee.Frames.AtCommands
{
    public class PrimitiveResponseData<TValue> : AtCommandResponseFrameData
    {
        public TValue Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
