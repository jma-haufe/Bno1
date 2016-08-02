using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    public interface IAdviceCategory
    {
        string Caption { get;  }

        List<IAdviceSubCategory> AdviceSubCategories { get; }
    }
}
