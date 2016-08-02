using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transmate.DataService;

namespace transmate.DataService
{
    public class Ticket
    {

        internal Ticket(Account account, Advice advice)
        {
            //Set references to other objects
            this.Account = account;
            this.Advice = advice;
            this.Office = advice.Office;
            //Set Timestamps
            PullDate = DateTime.Now;
            MaxValidUntil = this.Office.GetMaxOpeningToday();
            //Line Number
            LineNumber = ((Office)this.Office).GetNextNumberInLine(this);
            if (account != null)
            {
                account.MyTickets.Add(this);
            }
        }

        public Account Account { get; private set; }
        public Office Office { get; private set; }
        public Advice Advice { get; private set; }
        public int LineNumber { get; private set; }
        public DateTime PullDate { get; private set; }
        public DateTime MaxValidUntil { get; private set; }
        public int GetEstimatedWaitTimeInMinutes()
        {
            Office o = this.Office as Office;
            if (o != null)
            {
                return o.GetEstimatedWaitTimeInMinutes(this);
            }
            else
            {
                //Wrong implementation
                throw new ArgumentException(this.Office.GetType().Name + " is not supported");
            }
        }
    }
}
