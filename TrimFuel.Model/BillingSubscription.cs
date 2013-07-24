using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingSubscription : Entity
    {
        public int? BillingSubscriptionID { get; set; }
        public long? BillingID { get; set; }
        public int? SubscriptionID { get; set; }
        public DateTime? CreateDT { get; set; }
        public int? StatusTID { get; set; }
        public DateTime? LastBillDate { get; set; }
        public DateTime? NextBillDate { get; set; }
        //public string SKU { get; set; }
        public string CustomerReferenceNumber { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            //v.AssertString("SKU", SKU, 50);
            v.AssertString("CustomerReferenceNumber", CustomerReferenceNumber, 6);
        }
    }

    public class BillingSubscriptionList : List<BillingSubscription>
    {
    }
}
