using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XBee.Frames.AtCommands;
using XBee.Utility.ViewModels;

namespace XBee.Utility
{
    public class HardwareVersionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Series1DataTemplate { get; set; }
        public DataTemplate Series2DataTemplate { get; set; }
        public DataTemplate CellularDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            var hardware = (IHardware) item;

            switch (hardware.HardwareVersion)
            {
                case HardwareVersion.XBeeSeries1:
                case HardwareVersion.XBeeProSeries1:
                {
                    return Series1DataTemplate;
                }
                case HardwareVersion.XBeeProS2:
                case HardwareVersion.XBeeProS2B:
                case HardwareVersion.XBeeProS2C:
                case HardwareVersion.XBee24C:
                case HardwareVersion.XBeePro24C:
                    {
                    return Series2DataTemplate;
                }
                case HardwareVersion.XBeeCellular:
                {
                    return CellularDataTemplate;
                }
                default:
                {
                    return base.SelectTemplateCore(item, container);
                }
            }
        }
    }
}
