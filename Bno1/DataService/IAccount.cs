using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    /// <summary>
    /// Represent one user account. 
    /// </summary>
    public interface IAccount
    {
        String Name { get; set; }
        String TelephoneNumber { get; set; }
        String Password { get; set; }

        //TODO Ticktes
    }
}
