using System.ComponentModel;
using System.Runtime.CompilerServices;
using XBee.Util.Annotations;

namespace XBee.Util.ViewModels
{
    public abstract class NodeBase : INotifyPropertyChanged
    {
        private string _id;
        private NodeAddress _address;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public NodeAddress Address
        {
            get { return _address; }
            set
            {
                if (Equals(value, _address)) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
