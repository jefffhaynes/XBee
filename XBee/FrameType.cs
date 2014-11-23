namespace XBee
{
    public enum FrameType : byte
    {
        TxRequest = 0x00,
        Request16 = 0x01,
        AtCommand = 0x08,
        AtQueueCommand = 0x09,
        TxRequestExt = 0x10,
        TxRequestExplicit = 0x11,
        RemoteAtCommand = 0x17,
        RxIndicator = 0x80,
        AtCommandResponse = 0x88,
        TxStatus = 0x89,
        ModemStatus = 0x8a,
        TxStatusExt = 0x8b,
        RouteInformation = 0x8d,
        AggregateAddressingUpdate = 0x8e,
        RxIndicatorExt = 0x90,
        RxIndicatorExplicitExt = 0x91,
        DataSampleRxIndicator = 0x92,
        NodeIdentificationIndicator = 0x95,
        RemoteCommandResponse = 0x97,
        FirmwareUpdateStatus = 0xa0,
        RouteRecordIndicator = 0xa1,
        ManytooneRouteRequestIndicator = 0xa3,
        Unknown = 0xff,
    }
}
