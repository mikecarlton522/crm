using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class DeclineUpsell : Entity
    {
        public int? DeclineUpsellID { get; set; }
        public long? BillingID { get; set; }
        public long? ChargeHistoryID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingID", BillingID);
        }
    }
}
