using System;
using System.Windows;
using System.Windows.Controls;

namespace XBee.Util.Controls
{
    /// <summary>
    /// Interaction logic for XBeeGlyph.xaml
    /// </summary>
    public partial class XBeeGlyph : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (XBeeGlyph), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (XBeeGlyph)dependencyObject;
            control.XBeeTextBlock.Text = (string)dependencyPropertyChangedEventArgs.NewValue;
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public XBeeGlyph()
        {
            InitializeComponent();
        }
    }
}
