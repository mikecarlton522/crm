using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingReferralCoupon : FakeCoupon
    {
        public BillingReferralCoupon(long? billingID, decimal? discount)
            : base((billingID != null) ? billingID.Value.ToString() : string.Empty, discount)
        {
            BillingID = billingID;
        }

        public long? BillingID { get; set; }
    }
}
