using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SubscriptionPlan : Entity
    {
        public int? SubscriptionPlanID { get; set; }
        public string SubscriptionPlanName { get; set; }
        public int? StartSubscriptionPlanItemID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("SubscriptionPlanName", SubscriptionPlanName, 255);
            v.AssertNotNull("StartSubscriptionPlanItemID", StartSubscriptionPlanItemID);
        }
    }
}
