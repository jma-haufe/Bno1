using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transmate.DataService;

namespace transmate.DataService
{
    public class Office
    {
        private int _currentNumberInLine;
        private int _nextNumberInLine;
        private Queue<Ticket> _line;
        private object _syncRoot = new object();


        public Office()
        {
            OpeningHours = new List<OpeningHours>();
            _currentNumberInLine = 0;
            _nextNumberInLine = 1;
            _line =new Queue<Ticket>();
        }

        //[Key, Required]
        public Guid OfficeId { get; set; }
        //[Required]
        public string Name { get; set; }

        public string Street { get; set; }

        public string PLZ { get; set; }

        public string City { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }

        public string GetAdress()
        {
            return Street + "\r\n" + PLZ + " " + City;
        }
        public List<OpeningHours> OpeningHours { get; set; }


        public String GetOpeningHours()
        {
            OpeningHours.OrderBy(open => open.DayOfWeek).ThenBy(open => open.StartOpen);
            StringBuilder res = new StringBuilder();
            int lastDayOfWeek = -1;
            foreach (OpeningHours hours in OpeningHours)
            {
                if (lastDayOfWeek == hours.DayOfWeek)
                {
                    //SameDay
                    res.Append(" und ").Append(hours.StartOpen).Append(":00 bis ").Append(hours.EndOpen).Append(":00");
                    continue;
                } else if (lastDayOfWeek >= 0)
                {
                    res.AppendLine();
                }
                
                switch (hours.DayOfWeek)
                {
                    case 0:
                        res.Append("Sonntag ");
                        break;
                    case 1:
                        res.Append("Montag ");
                        break;
                    case 2:
                        res.Append("Dienstag ");
                        break;
                    case 3:
                        res.Append("Mittwoch ");
                        break;
                    case 4:
                        res.Append("Donnerstag ");
                        break;
                    case 5:
                        res.Append("Freitag ");
                        break;
                    case 6:
                        res.Append("Samstag ");
                        break;
                }
                res.Append(hours.StartOpen).Append(":00 bis ").Append(hours.EndOpen).Append(":00");
                
                lastDayOfWeek = hours.DayOfWeek;
            }
            return res.ToString();
        }

        public DateTime GetMaxOpeningToday()
        {
            DateTime now = DateTime.Now;
            int currentDayOfWeek = (int)now.DayOfWeek;
            int maxOpening = -1;
            foreach (OpeningHours hour in OpeningHours.FindAll(o => o.DayOfWeek == currentDayOfWeek))
            {
                if (hour.EndOpen > maxOpening)
                {
                    maxOpening = hour.EndOpen;
                }
            }
            if (maxOpening <= 0)
            {
                return DateTime.MinValue;
            }
            return new DateTime(now.Year, now.Month, now.Day, maxOpening > 23 ? 23 : maxOpening, 59, 59);
        }

        public bool IsOpen()
        {
            DateTime now = DateTime.Now;
            int currentDayOfWeek = (int)now.DayOfWeek;            
            foreach (OpeningHours hour in OpeningHours.FindAll(o => o.DayOfWeek == currentDayOfWeek))
            {
                if (hour.EndOpen >= now.Hour && hour.StartOpen <= now.Hour)
                {
                    return true;
                }
            }
            return false;
        }

        public DateTime GetNextOpeningToday()
        {
            DateTime now = DateTime.Now;
            DateTime nextOpen = DateTime.MaxValue;
            int currentDayOfWeek = (int)now.DayOfWeek;
            foreach (OpeningHours hour in OpeningHours.FindAll(o => o.DayOfWeek == currentDayOfWeek))
            {
                if (hour.EndOpen < now.Hour && hour.StartOpen > now.Hour)
                {
                    DateTime posNextOpen = new DateTime(now.Year, now.Month, now.Day, hour.StartOpen, 0, 0);
                    if (posNextOpen < nextOpen)
                    {
                        nextOpen = posNextOpen;
                    }
                }
            }
            return nextOpen;
        }

        internal int GetNextNumberInLine(Ticket ticket)
        {
            lock (_syncRoot)
            {
                int res = _nextNumberInLine;
                _nextNumberInLine++;
                _line.Enqueue(ticket);
                return res;
            }
        }

        public Ticket GetTicketForAdviceIfExists(Account a)
        {
            if (a == null) return null;
            foreach (Ticket ticket1 in _line)
            {
                if (ticket1.Account != null &&  ticket1.Account.AccountId.Equals(a.AccountId))
                {
                    return ticket1;
                }
            }
            return null;
        }

        public int GetEstimatedWaitTimeInMinutes(Ticket witingTicket)
        {
            int sumOfMinutes = 0;
            lock (_syncRoot)
            {
                foreach (Ticket ticket1 in _line)
                {
                    if (ticket1.LineNumber.Equals(witingTicket.LineNumber))
                    {
                        //Place in Line reached
                        break;
                    }
                    //Add to sum, Ticket is before the waiting ticket
                    sumOfMinutes += ticket1.Advice.DurationInMinutes;
                    sumOfMinutes++;//Time to switch to next
                }
            }
            return sumOfMinutes;
        }
        public int GetEstimatedWaitTimeInMinutes()
        {
            int sumOfMinutes = 0;
            lock (_syncRoot)
            {
                foreach (Ticket ticket1 in _line)
                {                    
                    //Add to sum, Ticket is before the waiting ticket
                    sumOfMinutes += ticket1.Advice.DurationInMinutes;
                    sumOfMinutes++;//Time to switch to next
                }
            }
            return sumOfMinutes;
        }

        public int GetTicketPosition(Ticket witingTicket)
        {
            int pos = 0;
            lock (_syncRoot)
            {
                foreach (Ticket ticket1 in _line)
                {
                    if (ticket1.LineNumber.Equals(witingTicket.LineNumber))
                    {
                        //Place in Line reached
                        break;
                    }
                    //Add to sum, Ticket is before the waiting ticket
                    pos++;
                }
            }
            return pos / 3;
        }
        public int GetCurrentWaiting()
        {
            return _line.Count / 3;
        }

        public Office Clone()
        {
            Office o = new Office()
            {
                Name = this.Name,
                City = this.City,
                Street = this.Street,
                PLZ = this.PLZ,
                Mail = this.Mail,
                Phone = this.Phone,
                OfficeId = this.OfficeId
            };
            var res = from oh in this.OpeningHours where !oh.IsMock select oh;
            o.OpeningHours = res.ToList();
            return o;
        }
    }
}
