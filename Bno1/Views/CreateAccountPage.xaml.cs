using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using transmate.DataService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Bno1.Views
{
    public sealed partial class CreateAccountPage : Page
    {
        public CreateAccountPage()
        {
            this.InitializeComponent();

            this.Loaded += CreateAccountPage_Loaded;

        }

        private void CreateAccountPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.signIn.Focus(FocusState.Programmatic);
        }
        
        private void OnCreateAccount(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.textName.Text.Length == 0)
            {
                this.textName.Background = new SolidColorBrush(Colors.LightPink);
                return;
            }
            
            //if (this.textPassword.Password.Length == 0)
            //{
            //    this.textPassword.Background = new SolidColorBrush(Colors.LightPink);
            //    return;
            //}

            if (DataService.Instance.HasUserWithName(this.textName.Text))
            {
                //Check Login
                if (DataService.Instance.TryLogin(this.textName.Text, this.textPassword.Password))
                {
                    Frame.Navigate(typeof(StartPage));
                }
                else
                {
                    this.textName.Background = new SolidColorBrush(Colors.LightPink);
                    return;
                }
            }
            //else if (this.textMail.Text.Length == 0)
            //{
            //    this.textMail.Background = new SolidColorBrush(Colors.LightPink);
            //    return;
            //}
            //else if (DataService.Instance.HasUserWithMail(this.textMail.Text))
            //{
            //    this.textMail.Background = new SolidColorBrush(Colors.LightPink);
            //    return;
            //}
            else
            {
                Frame.Navigate(typeof(WelcomePage), new Tuple<String, String, String>(textName.Text, textMail.Text, textPassword.Password));
            }
        }
    }
}
