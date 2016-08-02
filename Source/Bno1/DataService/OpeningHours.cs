using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using transmate.DataService;

namespace transmate.DataService
{
    public class OpeningHours
    {
        public int DayOfWeek { get; set; }
        public int StartOpen { get; set; }

        public int EndOpen { get; set; }

        [XmlIgnore]
        public bool IsMock { get; set; }
    }
}
