using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    /// <summary>
    /// Represents one ticket/number in one line at a office
    /// </summary>
    public interface ITicket
    {
        IAccount Account { get;  }
        IOffice Office { get;  }
        IAdvice Advice { get;  }

        int LineNumber { get;  }

        DateTime PullDate { get;  }

        DateTime MaxValidUntil { get; }

        int GetEstimatedWaitTimeInMinutes();
    }
}
