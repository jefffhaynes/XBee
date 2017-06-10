namespace XBee.Frames.AtCommands
{
    internal class PrimitiveResponseData<TValue> : AtCommandResponseFrameData
    {
        public TValue Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
