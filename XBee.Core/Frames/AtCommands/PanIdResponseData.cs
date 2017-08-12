using BinarySerialization;

namespace XBee.Frames.AtCommands
{
    internal class PanIdResponseData : AtCommandResponseFrameData
    {
        [FieldOrder(0)]
        [SerializeWhen("Protocol", XBeeProtocol.Raw, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhen("Protocol", XBeeProtocol.DigiMesh, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public ushort? Id { get; set; }

        [FieldOrder(1)]
        [SerializeWhenNot("Protocol", XBeeProtocol.Raw, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        [SerializeWhenNot("Protocol", XBeeProtocol.DigiMesh, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
        public ulong? IdExt { get; set; }
    }
}
