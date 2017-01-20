
using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    public class NodeIdentifierResponseData : AtCommandResponseFrameData
    {
        [FieldLength(20)]
        [FieldEncoding("ascii")]
        public string Id { get; set; }
    }
}
