using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using transmate.DataService;

namespace transmate.DataService
{
    public class Account
    {
        public Account()
        {
            MyTickets = new List<Ticket>();
        }

        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string TelephoneNumber { get; set; }
        public string Password { get; set; }
        [XmlIgnore]
        public List<Ticket> MyTickets { get; set; }
    }
}
