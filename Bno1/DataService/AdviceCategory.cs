using System;
using System.Collections.Generic;

namespace transmate.DataService
{
    public class AdviceCategory
    {
        public AdviceCategory()
        {
            AdviceSubCategories = new List<AdviceSubCategory>();
        }

        public Guid AdviceId { get; set; }

        public List<AdviceSubCategory> AdviceSubCategories { get; set; }
        
        public string Caption { get; set; }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}
