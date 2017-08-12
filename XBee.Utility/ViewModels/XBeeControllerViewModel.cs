using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Core;
using XBee.Frames.AtCommands;

namespace XBee.Utility.ViewModels
{
    public class XBeeControllerViewModel : ViewModelBase, IHardware
    {
        public XBeeControllerViewModel(Universal.XBeeController controller, HardwareVersion hardwareVersion, bool isCoordinator)
        {
            Controller = controller;
            HardwareVersion = hardwareVersion;
            IsCoordinator = isCoordinator;

            controller.NodeDiscovered += ControllerOnNodeDiscovered;

            DiscoverCommand = new RelayCommand(Discover);
        }

        private void ControllerOnNodeDiscovered(object sender, NodeDiscoveredEventArgs nodeDiscoveredEventArgs)
        {
            DispatchAsync(CoreDispatcherPriority.Normal, () => Nodes.Add(nodeDiscoveredEventArgs.Name));
        }

        public ObservableCollection<string> Nodes { get; } = new ObservableCollection<string>();
        public Universal.XBeeController Controller { get; }
        public HardwareVersion HardwareVersion { get; }
        public bool IsCoordinator { get; }

        public ICommand DiscoverCommand { get; }

        private async void Discover()
        {
            await Controller.DiscoverNetworkAsync();
        }
    }
}
