using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace XBee.Util.ViewModels
{
    public class Coordinator : NodeBase
    {
        public Coordinator(XBeeController controller)
        {
            Controller = controller;
            controller.NodeDiscovered += NodeDiscovered;
            DiscoverCommand = new RelayCommand(Discover);
        }

        private void NodeDiscovered(object sender, NodeDiscoveredEventArgs e)
        {
            var existingNode = EndPoints.SingleOrDefault(node => node.Address == e.Node.Address);
            if (existingNode != null)
            {
                existingNode.Id = e.Name;
            }
            else
            {
                var node = new EndPoint
                {
                    Address = e.Node.Address,
                    Id = e.Name
                };

                EndPoints.Add(node);
            }
        }

        public XBeeController Controller { get; }

        public ObservableCollection<EndPoint> EndPoints { get; set; } 

        public ICommand DiscoverCommand { get; }

        private async void Discover()
        {
            await Controller.DiscoverNetwork();
        }
    }
}
