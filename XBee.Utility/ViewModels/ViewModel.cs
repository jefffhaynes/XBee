using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using XBee.Devices;

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
            var serialDeviceTasks = devices.Select(device => TryGetSerialDeviceFromIdAsync(device.Id));
            var serialDevices = await Task.WhenAll(serialDeviceTasks);
            var serialDeviceViewModels = serialDevices.Where(serialDevice => serialDevice != null)
                .Select(serialDevice => new SerialDeviceViewModel(serialDevice))
                .ToList();

            var newDevices = serialDeviceViewModels.Except(SerialDevices).ToList();
            var missingDevices = SerialDevices.Except(serialDeviceViewModels).ToList();
            
            foreach (var serialDevice in newDevices)
            {
                SerialDevices.Add(serialDevice);
                serialDevice.SerialDevice.ReadTimeout = TimeSpan.MaxValue;
            }

            foreach (var missingDevice in missingDevices)
            {
                SerialDevices.Remove(missingDevice);
            }
        }

        private async Task<SerialDevice> TryGetSerialDeviceFromIdAsync(string id)
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
            var controller = new Universal.XBeeController(SelectedSerialDevice.SerialDevice);

            try
            {
                var hardwareVersion = await controller.GetHardwareVersionAsync();
                var node = controller.Local;

                bool isCoord = false;
                if (node is XBeeSeries1 series1)
                {
                    isCoord = await series1.IsCoordinatorAsync();
                }

                DiscoveredControllers.Add(new XBeeControllerViewModel(controller, hardwareVersion, isCoord));
            }
            catch (TimeoutException)
            {
            }
        }
    }
}