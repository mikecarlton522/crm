using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RefererBilling : Entity
    {
        public int? RefererBillingID { get; set; }
        public int? RefererID { get; set; }
        public long? BillingID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("RefererID", RefererID);
            v.AssertNotNull("BillingID", BillingID);
        }
    }
}
