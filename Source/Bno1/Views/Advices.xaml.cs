using System.Collections.Generic;
using System.Linq;
using transmate.DataService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Bno1.Views
{
    public sealed partial class Advices : Page
    {
        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set { SetValue(PageTitleProperty, value); }
        }
        public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(nameof(PageTitle), typeof(string), typeof(Advices), new PropertyMetadata(""));

        private Advice _myAdvice;
        
        public Advices()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _myAdvice = e.Parameter as Advice;
            ShowAdvices();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAdvices();
        }

        private void AdviceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _myAdvice = AdviceListView.SelectedItem as Advice;
            if (_myAdvice == null) return;    
            
            if (_myAdvice.Advices.Count > 0)
            {
                ShowAdvices();
            }
            else
            {
                Advice a = AdviceListView.SelectedItem as Advice;
                if (a != null)
                {
                    Frame.Navigate(typeof(SingleAdvicePage), AdviceListView.SelectedItem as Advice);
                }
            }
        }

        private void ShowAdvices()
        {
            if (_myAdvice != null)
            {
                this.titlePathBox.TitlePath += " > " + _myAdvice.Caption;
                this.PageTitle = _myAdvice.Caption;
                List<Advice> accounts = _myAdvice.Advices;

                if (accounts.Any())
                {
                    AdviceListView.ItemsSource = accounts;
                    AdviceListView.SelectionChanged += AdviceListView_SelectionChanged;
                }
            }
            else
            {
                this.titlePathBox.TitlePath = " > Advice";
                this.PageTitle = "Categories";
                List<Advice> accounts = DataService.Instance.GetAllAdviceCategories();

                if (accounts.Any())
                {
                    AdviceListView.ItemsSource = accounts;
                    AdviceListView.SelectionChanged += AdviceListView_SelectionChanged;
                }
            }
        }

        private void OnHomeTapped(object sender, System.EventArgs e)
        {
            Frame.Navigate(typeof(Views.StartPage));
        }
    }
}
