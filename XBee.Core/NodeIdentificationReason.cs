using JetBrains.Annotations;

namespace XBee
{
    [PublicAPI]
    public enum NodeIdentificationReason : byte
    {
        Button = 1,
        Joining = 2,
        Power = 3
    }
}
