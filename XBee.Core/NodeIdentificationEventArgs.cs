using System;
using JetBrains.Annotations;
using XBee.Frames;
using XBee.Frames.AtCommands;

namespace XBee
{
    [PublicAPI]
    public class NodeIdentificationEventArgs : EventArgs
    {
        public NodeIdentificationEventArgs(NodeAddress senderAddress, NodeAddress remoteAddress,
            ShortAddress parentAddress, string name, DeviceType deviceType,
            NodeIdentificationReason nodeIdentificationReason, ReceiveOptionsExt receiveOptions, 
            ushort digiProfileId, ushort manufacturerId)
        {
            SenderAddress = senderAddress;
            RemoteAddress = remoteAddress;
            ParentAddress = parentAddress;
            Name = name;
            DeviceType = deviceType;
            NodeIdentificationReason = nodeIdentificationReason;
            ReceiveOptions = receiveOptions;
            DigiProfileId = digiProfileId;
            ManufacturerId = manufacturerId;
        }

        public NodeAddress SenderAddress { get; }

        public NodeAddress RemoteAddress { get; }

        public ShortAddress ParentAddress { get; }

        public string Name { get; }

        public DeviceType DeviceType { get; }

        public NodeIdentificationReason NodeIdentificationReason { get; }

        public ReceiveOptionsExt ReceiveOptions { get; }
        
        public ushort DigiProfileId { get; }
        
        public ushort ManufacturerId { get; }
    }
}
