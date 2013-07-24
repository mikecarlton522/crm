using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingSubscriptionRebillDiscount : Entity
    {
        public int? DiscountTypeID { get; set; }
        public int? BillingSubscriptionID { get; set; }
        public decimal? DiscountValue { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("DiscountTypeID", DiscountTypeID);
            v.AssertNotNull("BillingSubscriptionID", BillingSubscriptionID);
            v.AssertNotNull("DiscountValue", DiscountValue);
        }
    }
}
