using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    /// <summary>
    /// Represents one line/item in a checklist
    /// </summary>
    public interface ICheckListItem
    {
        String Name { get;  }
        /// <summary>
        /// Optional Link to a Document, if null or empty no document is required for this item
        /// </summary>
        String Link { get;  }
    }
}
