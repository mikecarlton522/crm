using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingSubscriptionPlan : Entity
    {
        public int? BillingSubscriptionPlanID { get; set; }
        public int? BillingSubscriptionID { get; set; }
        public long? OrderRecurringPlanID { get; set; }
        public int? LastItemID { get; set; }
        public DateTime? LastItemDate { get; set; }
        public int? NextItemID { get; set; }
        public DateTime? NextItemDate { get; set; }
        public int? SubscriptionPlanID { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool IsActive { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            //v.AssertNotNull("BillingSubscriptionID", BillingSubscriptionID);
            v.AssertNotNull("LastItemID", LastItemID);
            v.AssertNotNull("LastItemDate", LastItemDate);
            v.AssertNotNull("SubscriptionPlanID", SubscriptionPlanID);
            //v.AssertNotNull("NextItemID", NextItemID);
            //v.AssertNotNull("NextItemDate", NextItemDate);
            v.AssertNotNull("CreateDT", CreateDT);
        }
    }
}
