using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    public interface IAdvice
    {
        String Caption { get; }
        String Text { get; }
        IOffice Office { get; }
        int DurationInMinutes { get; }

        List<ICheckListItem> CheckListItems
        {
            get;
        }
    }
}
