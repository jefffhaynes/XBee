namespace XBee.Frames.AtCommands
{
    public class StringResponseData : PrimitiveResponseData<string>
    {
        public override string ToString()
        {
            return Value;
        }
    }
}
