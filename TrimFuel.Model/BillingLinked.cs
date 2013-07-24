using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingLinked : Entity
    {
        public int? BillingLinkedID { get; set; }
        public int? ParentBillingID { get; set; }
        public int? BillingID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
