using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    /// <summary>
    /// Represents one timeslot a office is open
    /// </summary>
    public interface IOpeningHour
    {
        int DayOfWeek { get;  }
        int StartOpen { get;  }
        int EndOpen { get;  }
    }
}
