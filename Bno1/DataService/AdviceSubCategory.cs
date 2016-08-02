using System;
using System.Collections.Generic;

namespace transmate.DataService
{
    public class AdviceSubCategory
    {
        public AdviceSubCategory()
        {
            Advices = new List<Advice>();
        }

        public Guid AdviceId { get; set; }
        
        public string Caption { get; set; }

        public List<Advice> Advices { get; set; }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}
