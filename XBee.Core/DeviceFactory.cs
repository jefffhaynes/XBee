using System;
using System.Collections.Generic;
using XBee.Core;
using XBee.Devices;
using XBee.Frames.AtCommands;

namespace XBee
{
    internal static class DeviceFactory
    {
        private static readonly Dictionary<HardwareVersion, DeviceSeries> Series = new Dictionary<HardwareVersion, DeviceSeries>
        {
            {HardwareVersion.XBeeSeries1, DeviceSeries.Series1},
            {HardwareVersion.XBeeProSeries1, DeviceSeries.Series1},
            {HardwareVersion.ZNetZigBeeS2, DeviceSeries.Series2},
            {HardwareVersion.XBeeProS2, DeviceSeries.Series2},
            {HardwareVersion.XBeeProS2B, DeviceSeries.Series2},
            {HardwareVersion.XBee24S2C, DeviceSeries.Series2},
            {HardwareVersion.XBee24C, DeviceSeries.Series2},
            {HardwareVersion.XBeePro24C, DeviceSeries.Series2},
            {HardwareVersion.XBeePro24CSmt, DeviceSeries.Series2},
            {HardwareVersion.XBeePro900, DeviceSeries.Pro900},
            {HardwareVersion.XBeePro900HP, DeviceSeries.Pro900},
            {HardwareVersion.XBeeProSX, DeviceSeries.Pro900},
            {HardwareVersion.XBeeCellular, DeviceSeries.Cellular}
        };

        public static DeviceSeries GetSeries(HardwareVersion hardwareVersion)
        {
            DeviceSeries series;
            if (!Series.TryGetValue(hardwareVersion, out series))
            {
                throw new NotSupportedException($"Hardware version {hardwareVersion} not supported.");
            }

            return series;
        }

        public static XBeeNode CreateDevice(HardwareVersion hardwareVersion, ushort firmwareVersion, NodeAddress address, XBeeControllerBase controller)
        {
            var series = GetSeries(hardwareVersion);
            var protocol = GetProtocol(hardwareVersion, firmwareVersion);

            switch (series)
            {
                case DeviceSeries.Series1:
                    return new XBeeSeries1(controller, hardwareVersion, firmwareVersion, protocol, address);
                case DeviceSeries.Series2:
                    return new XBeeSeries2(controller, hardwareVersion, firmwareVersion, protocol, address);
                case DeviceSeries.Pro900:
                    return new XBeePro900HP(controller, hardwareVersion, firmwareVersion, protocol, address);
                case DeviceSeries.Cellular:
                    return new XBeeCellular(controller, hardwareVersion, firmwareVersion, protocol, address);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// This is a direct port from the Digi library b/c it's so frickin complicated.  It's called standards Digi.
        /// </summary>
        /// <param name="hardwareVersion"></param>
        /// <param name="firmwareVersion"></param>
        /// <returns></returns>
        public static XBeeProtocol GetProtocol(HardwareVersion hardwareVersion, ushort firmwareVersion)
        {
            var firmwareVersionHex = firmwareVersion.ToString("X");

            switch (hardwareVersion)
            {
                case HardwareVersion.XBeeSeries1:
                case HardwareVersion.XBeeProSeries1:
                    {
                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("8"))
                        {
                            return XBeeProtocol.DigiMesh;
                        }

                        return XBeeProtocol.Raw;
                    }

                case HardwareVersion.ZNetZigBeeS2:
                case HardwareVersion.XBeeProS2:
                    {
                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("1") && firmwareVersionHex.EndsWith("20")
                            || firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("2"))
                            return XBeeProtocol.ZigBee;

                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("3"))
                            return XBeeProtocol.SmartEnergy;

                        return XBeeProtocol.ZNet;
                    }

                case HardwareVersion.XBeePro900:
                    {
                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("8")
                            || firmwareVersionHex.Length == 4 && firmwareVersionHex[1] == '8'
                            || firmwareVersionHex.Length == 5 && firmwareVersionHex[1] == '8')
                            return XBeeProtocol.DigiMesh;
                        return XBeeProtocol.DigiPoint;
                    }

                case HardwareVersion.XBeePro868:
                    return XBeeProtocol.DigiPoint;
                case HardwareVersion.XBeeProS2B:
                    {
                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("3"))
                            return XBeeProtocol.SmartEnergy;
                        return XBeeProtocol.ZigBee;
                    }

                case HardwareVersion.XBeeProS2C:
                case HardwareVersion.XBee24S2C:
                    {
                        if (firmwareVersionHex.Length == 4 &&
                            (firmwareVersionHex.StartsWith("5") || firmwareVersionHex.StartsWith("6")))
                            return XBeeProtocol.SmartEnergy;
                        if (firmwareVersionHex.StartsWith("2"))
                            return XBeeProtocol.Raw;
                        if (firmwareVersionHex.StartsWith("9"))
                            return XBeeProtocol.DigiMesh;
                        return XBeeProtocol.ZigBee;
                    }

                case HardwareVersion.XBeePro900HP:
                    {
                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("8"))
                            return XBeeProtocol.DigiMesh;
                        if (firmwareVersionHex.Length == 4 && firmwareVersionHex.StartsWith("1"))
                            return XBeeProtocol.DigiPoint;
                        return XBeeProtocol.XC;
                    }

                case HardwareVersion.XBeeProSX:
                    {
                        if (firmwareVersionHex.StartsWith("2"))
                            return XBeeProtocol.XTend;
                        if (firmwareVersionHex.StartsWith("8"))
                            return XBeeProtocol.XTendDigiMesh;
                        return XBeeProtocol.SX;
                    }

                case HardwareVersion.XBee24C:
                case HardwareVersion.XBeePro24C:
                case HardwareVersion.XBeePro24CSmt:
                    {
                        if (firmwareVersionHex.Length == 4 &&
                            (firmwareVersionHex.StartsWith("5") || firmwareVersionHex.StartsWith("6")))
                            return XBeeProtocol.SmartEnergy;
                        if (firmwareVersionHex.StartsWith("2"))
                            return XBeeProtocol.Raw;
                        if (firmwareVersionHex.StartsWith("9"))
                            return XBeeProtocol.DigiMesh;
                        return XBeeProtocol.ZigBee;
                    }

                case HardwareVersion.XBeeCellular:
                    return XBeeProtocol.Cellular;


            }

            throw new NotSupportedException($"Hardware version {hardwareVersion} not supported.");
        }


    }
}
