using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using XBee.Devices;
using XBee.Universal;

namespace XBee.Utility.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        private SerialDeviceViewModel _selectedSerialDevice;
        private IHardware _selectedHardware;
        public ObservableCollection<SerialDeviceViewModel> SerialDevices { get; } = new ObservableCollection<SerialDeviceViewModel>();

        public ViewModel()
        {
            ConnectCommand = new RelayCommand(ConnectAsync);
        }

        public SerialDeviceViewModel SelectedSerialDevice
        {
            get => _selectedSerialDevice;

            set
            {
                if (Equals(value, _selectedSerialDevice)) return;
                _selectedSerialDevice = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<XBeeControllerViewModel> DiscoveredControllers { get; } = new ObservableCollection<XBeeControllerViewModel>();

        public IHardware SelectedHardware
        {
            get => _selectedHardware;

            set
            {
                if (Equals(value, _selectedHardware)) return;
                _selectedHardware = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand { get; }

        public async Task RefreshDevicesAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
            foreach (var device in devices)
            {
                var serialDevice = await TryGetSerialDeviceFromIdAsync(device.Id);

                if (serialDevice != null)
                {
                    var serialDeviceViewModel = new SerialDeviceViewModel(serialDevice);
                    SerialDevices.Add(serialDeviceViewModel);
                }
            }
        }

        private static async Task<SerialDevice> TryGetSerialDeviceFromIdAsync(string id)
        {
            try
            {
                return await SerialDevice.FromIdAsync(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        private async void ConnectAsync()
        {
            var controller = new XBeeController(SelectedSerialDevice.SerialDevice);

            try
            {
                var hardwareVersion = await controller.GetHardwareVersionAsync();
                var node = controller.Local;

                bool isCoord = false;
                if (node is XBeeSeries1 series1)
                {
                    isCoord = await series1.IsCoordinatorAsync();
                    await series1.SetPanIdAsync(3535);
                    //var rp = await series1.GetPullUpResistorConfigurationAsync();
                    //var panId = await series1.GetPanIdAsync();
                }

                if (node is XBeeSeries2 series2)
                {
                    await series2.SetRssiPwmTimeAsync(33);
                    //var sleepOptions = await series2.GetSleepOptionsAsync();
                    //var association = await series2.GetAssociationAsync();
                    //var baudRate = await series2.GetBaudRateAsync();
                    //var panId = await series2.GetPanIdAsync();
                    //var rp = await series2.GetPullUpResistorConfigurationAsync();
                    //var pr = await series2.GetRssiPwmTimeAsync();
                    //var cd = await series2.GetChangeDetectionChannelsAsync();
                }

                DiscoveredControllers.Add(new XBeeControllerViewModel(controller, hardwareVersion, isCoord));
            }
            catch (TimeoutException)
            {
                controller.Dispose();
            }
        }
    }
}