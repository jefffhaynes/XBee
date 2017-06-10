namespace XBee.Frames.AtCommands
{
    internal class StringResponseData : PrimitiveResponseData<string>
    {
        public override string ToString()
        {
            return Value;
        }
    }
}
