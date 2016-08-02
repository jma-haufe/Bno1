using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    public interface IOffice
    {
        string Name { get;  }

        string ContactInfo { get; }


        List<IOpeningHour> OpeningHours { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Opening Hours as Human readable String</returns>
        String GetOpeningHours();
        DateTime GetMaxOpeningToday();
        
    }
}
