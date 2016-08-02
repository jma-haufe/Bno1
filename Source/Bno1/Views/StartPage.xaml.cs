using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Bno1.View;

namespace Bno1.Views
{
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private void OnOpenAdvice(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Advices));
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QRPage));
        }
    }
}
