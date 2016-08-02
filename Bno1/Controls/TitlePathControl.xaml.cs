using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bno1.Controls
{
    public sealed partial class TitlePathControl : UserControl
    {
        public event EventHandler<EventArgs> HomeTapped;

        public string TitlePath
        {
            get { return (string)GetValue(TitlePathProperty); }
            set { SetValue(TitlePathProperty, value); }
        }
        public static readonly DependencyProperty TitlePathProperty = DependencyProperty.Register(nameof(TitlePath), typeof(string), typeof(TitlePathControl), new PropertyMetadata(string.Empty));
        
        public TitlePathControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void OnHomeTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.HomeTapped?.Invoke(this, EventArgs.Empty);
        }
    }
}
