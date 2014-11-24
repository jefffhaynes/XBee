
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NodeIdentifierResponseData : AtCommandResponseFrameData
    {
        [FieldLength(20)]
        [SerializeAs(Encoding = "ascii")]
        public string Id { get; set; }
    }
}
