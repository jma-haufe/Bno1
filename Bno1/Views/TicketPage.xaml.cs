using System;
using System.Linq;
using transmate.DataService;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Bno1.Views
{
    public sealed partial class TicketPage : Page
    {
        private Ticket _ticket;

        public TicketPage()
        {
            this.InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Ticket ticket = e.Parameter as Ticket;
                if (ticket != null)
                {
                    _ticket = ticket;
                    this.textBlockNumber.Text = String.Format("# {0:D3}", ticket.LineNumber);
                    Office o = ticket.Office;
                    if (o != null)
                    {
                        textBlockAdress.Text = o.Street + ", " + o.PLZ + ", " + o.City;
                        UpdateWaitTime();
                    }
                }
            }
            else if (DataService.Instance.CurrentAccount!=null)
            {
                Ticket ticket = DataService.Instance.CurrentAccount.MyTickets.FirstOrDefault();
                if (ticket != null)
                {
                    _ticket = ticket;
                    this.textBlockNumber.Text = String.Format("# {0:D3}", ticket.LineNumber);
                    Office o = ticket.Office;
                    if (o != null)
                    {
                        textBlockAdress.Text = o.Street + ", " + o.PLZ + ", " + o.City;
                        UpdateWaitTime();
                    }
                }
            }
        }

        private void UpdateWaitTime()
        {
            if (this._ticket != null)
            {
                this.textBlockNumber.Text = String.Format("# {0:D3}", _ticket.LineNumber);
                this.textBlockStatusTime.Text = "Status updated: " + DateTime.Now.ToString("h:mm tt");
                Office o = _ticket.Office;
                if (o != null)
                {
                    TimeSpan ts = o.GetMaxOpeningToday() - DateTime.Now;
                    if (ts.TotalMinutes > 0)
                    {
                        this._progress.Maximum = ts.TotalMinutes;
                        var estimatedWaitTimeInMinutes = o.GetEstimatedWaitTimeInMinutes(_ticket);
                        this._progress.Value = estimatedWaitTimeInMinutes;
                        this.textBlockExpectedNumber.Text = (o.GetTicketPosition(_ticket)).ToString();

                        if (o.IsOpen())
                        {
                            this.textBlockExpectedTime.Text =
                                DateTime.Now.AddMinutes(estimatedWaitTimeInMinutes).ToString("h:mm tt");
                        }
                        else
                        {
                            this.textBlockExpectedTime.Text =
                                o.GetNextOpeningToday().AddMinutes(estimatedWaitTimeInMinutes).ToString("h:mm tt");
                        }
                    }
                    else
                    {
                        this._progress.Maximum = 100;
                        this._progress.Value = 100;
                        this._progress.Foreground = new SolidColorBrush(Colors.DarkRed);
                        this.textBlockExpectedNumber.Text = "Closed";
                    }
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            UpdateWaitTime();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (_ticket != null)
            {
                Frame.Navigate(typeof(SingleAdvicePage), _ticket.Advice);
            }
        }
    }

}
