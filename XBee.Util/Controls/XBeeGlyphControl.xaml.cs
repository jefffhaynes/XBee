using System;
using System.Windows;
using System.Windows.Controls;

namespace XBee.Util.Controls
{
    public partial class XBeeGlyphControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (XBeeGlyphControl), new PropertyMetadata("?", PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (XBeeGlyphControl)dependencyObject;
            control.XBeeTextBlock.Text = (string)dependencyPropertyChangedEventArgs.NewValue;
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public XBeeGlyphControl()
        {
            InitializeComponent();
        }
    }
}
