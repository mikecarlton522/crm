using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingSubscriptionDiscount : Entity
    {
        public int? BillingSubscriptionID { get; set; }
        public bool? IsSavePrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NewShippingAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            //BillingSubscription 1 <-> 0..1 BillingSubscriptionDiscount association
            v.AssertNotNull("BillingSubscriptionID", BillingSubscriptionID);
            v.AssertNotNull("IsSavePrice", IsSavePrice);
        }
    }
}
