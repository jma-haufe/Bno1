using System;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using transmate.DataService;

namespace Bno1.Views
{
    public sealed partial class WelcomePage : Page
    {
        private Tuple<string, string, string> _values;

        public WelcomePage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.Parameter != null)
            {
                _values = e.Parameter as Tuple<String, String, String>;
                if (_values != null)
                {
                    textWelcome.Text = "Welcome, " + _values.Item1 + "!";
                }
            }
        }

        private void OnSaveAndStart(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                //if (textPhone.Text.Length == 0)
                //{
                //    textPhone.Background = new SolidColorBrush(Colors.LightPink);
                //    return;
                //}
                DataService.Instance.AddUser(_values.Item1, textPhone.Text, _values.Item2, _values.Item3);
                Frame.Navigate(typeof(StartPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Frame.Navigate(typeof(StartPage));
            }
        }
    }
}
