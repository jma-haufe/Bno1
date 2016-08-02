using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    public interface IAdviceSubCategory
    {
        string Caption { get;  }

        List<IAdvice> Advices { get; }
    }
}
