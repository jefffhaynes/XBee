using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.SerialCommunication;

namespace XBee.Utility.ViewModels
{
    public class SerialDeviceViewModel : ViewModelBase
    {
        private readonly List<uint> _supportedBaudRates = new List<uint> {1200, 9600, 38400, 57600, 115200};

        public SerialDeviceViewModel(SerialDevice serialDevice)
        {
            SerialDevice = serialDevice;
        }

        public SerialDevice SerialDevice { get; }
        
        public string Name => SerialDevice.PortName;

        public IList<uint> SupportedBaudRates => _supportedBaudRates;

        public uint BaudRate
        {
            get => SerialDevice.BaudRate;

            set
            {
                if (value == SerialDevice.BaudRate) return;
                SerialDevice.BaudRate = value;
                OnPropertyChanged();
            }
        }

        public IList<ushort> SupportedDataBits => new List<ushort> {7, 8};

        public ushort DataBits
        {
            get => SerialDevice.DataBits;
            set
            {
                if (value == SerialDevice.DataBits) return;
                SerialDevice.DataBits = value;
                OnPropertyChanged();
            }
        }

        public IList<SerialStopBitCount> SupportedStopBits => Enum.GetValues(typeof(SerialStopBitCount)).Cast<SerialStopBitCount>()
            .ToList();

        public SerialStopBitCount StopBits
        {
            get => SerialDevice.StopBits;
            set
            {
                if (value == SerialDevice.StopBits) return;
                SerialDevice.StopBits = value;
                OnPropertyChanged();
            }
        }

        public IList<SerialParity> SupportedParity => Enum.GetValues(typeof(SerialParity)).Cast<SerialParity>()
            .ToList();

        public SerialParity Parity
        {
            get => SerialDevice.Parity;
            set
            {
                if (value == SerialDevice.Parity) return;
                SerialDevice.Parity = value;
                OnPropertyChanged();
            }
        }

        public IList<SerialHandshake> SupportedHandshakes => Enum.GetValues(typeof(SerialHandshake)).Cast<SerialHandshake>()
            .ToList();

        public SerialHandshake Handshake
        {
            get => SerialDevice.Handshake;
            set
            {
                if (value == SerialDevice.Handshake) return;
                SerialDevice.Handshake = value;
                OnPropertyChanged();
            }
        }
    }
}
