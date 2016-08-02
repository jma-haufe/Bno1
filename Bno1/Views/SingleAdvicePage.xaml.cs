using System;
using System.Collections.ObjectModel;
using System.Linq;
using transmate.DataService;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Bno1.Views
{
    public sealed partial class SingleAdvicePage : Page
    {
        public ObservableCollection<CheckListItem> CheckListItems { get; set; } = new ObservableCollection<CheckListItem>();

        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set { SetValue(PageTitleProperty, value); }
        }
        public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(nameof(PageTitle), typeof(string), typeof(Advices), new PropertyMetadata(string.Empty));
        
        public string CheckListHeader { get { return "You'll need following documents for this appointment"; } }

        public string CheckListFooter { get { return "Please confirm that you're able to submit those documents to get."; } }

        public string FeeInfo { get { return _advice?.Fees; } }

        public string ConfirmationText { get { return "I confirm that I can be on estimated time on spot"; } }

        public bool EnableQueueNumberService
        {
            get { return (bool)GetValue(EnableQueueNumberServiceProperty); }
            set { SetValue(EnableQueueNumberServiceProperty, value); }
        }
        public static readonly DependencyProperty EnableQueueNumberServiceProperty = DependencyProperty.Register(nameof(EnableQueueNumberService), typeof(bool), typeof(SingleAdvicePage), new PropertyMetadata(false));

        public bool ConfirmationAccepted
        {
            get { return (bool)GetValue(ConfirmationAcceptedProperty); }
            set { SetValue(ConfirmationAcceptedProperty, value); }
        }
        public static readonly DependencyProperty ConfirmationAcceptedProperty = DependencyProperty.Register(nameof(ConfirmationAccepted), typeof(bool), typeof(SingleAdvicePage), new PropertyMetadata(false));

        public bool ConfirmationEnabled
        {
            get { return (bool)GetValue(ConfirmationEnabledProperty); }
            set { SetValue(ConfirmationEnabledProperty, value); }
        }
        public static readonly DependencyProperty ConfirmationEnabledProperty = DependencyProperty.Register(nameof(ConfirmationEnabled), typeof(bool), typeof(SingleAdvicePage), new PropertyMetadata(false));

        public string ExpectedNumberInLineText { get; set; } = "Your expected number in line: ";

        public string ExpectedNumber
        {
            get { return (string)GetValue(ExpectedNumberProperty); }
            set { SetValue(ExpectedNumberProperty, value); }
        }
        public static readonly DependencyProperty ExpectedNumberProperty = DependencyProperty.Register(nameof(ExpectedNumber), typeof(string), typeof(SingleAdvicePage), new PropertyMetadata(string.Empty));

        public string ExpectedTime
        {
            get { return (string)GetValue(ExpectedTimeProperty); }
            set { SetValue(ExpectedTimeProperty, value); }
        }
        public static readonly DependencyProperty ExpectedTimeProperty = DependencyProperty.Register(nameof(ExpectedTime), typeof(string), typeof(SingleAdvicePage), new PropertyMetadata(string.Empty));

        public bool GetNumberAllowed { get; set; } = false;

        public string OfficeName { get { return _office?.Name; } }
        public string OfficeSubCaption { get { return "Community Service Department"; } }
        public string OfficeAddress { get { return _office?.GetAdress(); } }
        public string OfficePhoneNumber { get { return _office?.Phone; } }
        public string OfficeEmail { get { return _office?.Mail; } }
        public string OfficeOpening { get { return _office?.GetOpeningHours(); } }
        public string OfficeInfo { get { return GetOfficeInfo(); } }

        private string GetOfficeInfo()
        {
            return "General Information: " + Environment.NewLine
                + "Registration, re - registration and deregistration of residence, "
                + "EU citizens will be redirected right after registration "
                + "and receive a 'Freizügigkeitsbescheinigung' "
                + "(freedom of movement certificate) there.";
        }

        private Office _office;
        private Advice _advice;
        private Ticket _ticket;
        private static string baseUri = "bingmaps:?";

        public SingleAdvicePage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Advice advice = e.Parameter as Advice;
                this.titlePathBox.TitlePath = advice?.Caption;
                if (advice != null)
                {
                    this._advice = advice;
                    this.PageTitle = advice.Caption;
                    Office o = advice.Office;
                    if (o != null)
                    {
                        this._office = o;
                        
                        TimeSpan ts = o.GetMaxOpeningToday() - DateTime.Now;
                        if (ts.TotalMinutes > 0)
                        {
                            this._progress.Maximum = ts.TotalMinutes;
                            this._progress.Value = 10 + o.GetEstimatedWaitTimeInMinutes();
                            this.ExpectedNumber = (o.GetCurrentWaiting()).ToString();
                            this.ExpectedTime = DateTime.Now.AddMinutes(10 + o.GetEstimatedWaitTimeInMinutes()).ToString("h:mm tt");
                        }
                        else
                        {
                            this._progress.Maximum = 100;
                            this._progress.Value = 100;
                            this._progress.Foreground = new SolidColorBrush(Colors.DarkRed);
                            this.ExpectedNumber = "Closed";
                        }
                    }
                    foreach (CheckListItem item in advice.CheckListItems)
                    {
                        this.CheckListItems.Add(item);
                    }
                    
                    bool allChecked = CheckListItems.All(checkListItem => checkListItem.IsChecked);

                    if (!allChecked)
                    {
                        this._officeGrid.Visibility = Visibility.Visible;
                        this._itemGrid.Visibility = Visibility.Collapsed;
                        this._lineGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this._officeGrid.Visibility = Visibility.Collapsed;
                        this._itemGrid.Visibility = Visibility.Visible;
                        this._lineGrid.Visibility = Visibility.Collapsed;
                        this.EnableQueueNumberService = true;
                        if (o != null)
                        {
                            this._ticket = o.GetTicketForAdviceIfExists(DataService.Instance.CurrentAccount);
                            LoadTicket();
                        } 
                    }
                }
            }
        }

        private void LoadTicket()
        {
            if (this._ticket == null || this._office==null)
            {
                //Nothing found
                this.ExpectedNumberInLineText = "Your expected number in line: ";
                this.getNumberButton.Content = "Request a number now";
                this.ConfirmationAccepted = false;
                this.ConfirmationEnabled = true;
            }
            else
            {
                this.ExpectedNumberInLineText = "Your number in line: ";
                this.getNumberButton.Content = "Show Line";
                this.ConfirmationAccepted = true;
                this.ConfirmationEnabled = false;
                TimeSpan ts = _office.GetMaxOpeningToday() - DateTime.Now;
                if (ts.TotalMinutes > 0)
                {
                    int minutes = _office.GetEstimatedWaitTimeInMinutes(this._ticket);
                    this._progress.Maximum = ts.TotalMinutes;
                    this._progress.Value = 10 + minutes;
                    this.ExpectedNumber = (_ticket.LineNumber).ToString();
                    this.ExpectedTime = DateTime.Now.AddMinutes(10 + minutes).ToString("h:mm tt");
                }
                else
                {
                    this._progress.Maximum = 100;
                    this._progress.Value = 100;
                    this._progress.Foreground = new SolidColorBrush(Colors.DarkRed);
                    this.ExpectedNumber = "Closed";
                }
            }
        }

        private void OnOpenOfficeInfo(object sender, RoutedEventArgs e)
        {
            this._officeGrid.Visibility = Visibility.Visible;
            this._itemGrid.Visibility = Visibility.Collapsed;
            this._lineGrid.Visibility = Visibility.Collapsed;
        }
        private void OnOpenChecklist(object sender, RoutedEventArgs e)
        {
            this._officeGrid.Visibility = Visibility.Collapsed;
            this._itemGrid.Visibility = Visibility.Visible;
            this._lineGrid.Visibility = Visibility.Collapsed;
        }
        private void OnOpenQueueNumberService(object sender, RoutedEventArgs e)
        {
            this._officeGrid.Visibility = Visibility.Collapsed;
            this._itemGrid.Visibility = Visibility.Collapsed;
            this._lineGrid.Visibility = Visibility.Visible;
        }

        private void OnCheckListTapped(object sender, TappedRoutedEventArgs e)
        {
            bool allChecked = CheckListItems.All(checkListItem => checkListItem.IsChecked);
            this.EnableQueueNumberService = allChecked;
        }
        
        private void OnConfirmationChecked(object sender, RoutedEventArgs e)
        {
            if (_advice != null)
            {
                this.getNumberButton.IsEnabled = true;
            }
        }

        private void OnConfirmationUnchecked(object sender, RoutedEventArgs e)
        {
            this.getNumberButton.IsEnabled = false;
        }

        private void OnGetNumber(object sender, RoutedEventArgs e)
        {
            if (_advice != null)
            {
                Ticket t = DataService.Instance.PullTicketForAdvice(null, _advice);
                if (t != null)
                {
                    Frame.Navigate(typeof(TicketPage), t);
                }
            }
        }

        private async void OnDownloadClicked(object sender, RoutedEventArgs e)
        {
            var uri = new System.Uri("https://drive.google.com/open?id=0B_eDInKw2zx2MnpiODRjUmVac2M");
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri);           
        }

        private async void OnCheckArrivalClicked(object sender, RoutedEventArgs e)
        {
            if (this._office != null)
            {
                string uriStr = string.Format("{0}rtp=adr.{1}~adr.{2}", baseUri, Uri.EscapeDataString("Eisenbahnstraße 58-62, 79098 Freiburg"), Uri.EscapeDataString(_office.Street+", "+ _office.PLZ + " " + _office.City));

                var uri = new Uri(uriStr);
                bool success = await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        private void OnHomeTapped(object sender, System.EventArgs e)
        {
            Frame.Navigate(typeof(Views.StartPage));
        }
    }
}
