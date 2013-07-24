using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingSubscriptionStatusHistory : Entity
    {
        public int? BillingSubscriptionStatusHistoryID { get; set; }
        public int? BillingSubscriptionID { get; set; }
        public int? StatusTID { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingSubscriptionID", BillingSubscriptionID);
            v.AssertNotNull("BillingSubscriptionID", BillingSubscriptionID);
            v.AssertNotNull("StatusTID", StatusTID);
            v.AssertNotNull("CreateDT", CreateDT);
        }
    }
}
