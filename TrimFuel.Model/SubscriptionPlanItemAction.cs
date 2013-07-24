using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SubscriptionPlanItemAction : Entity
    {
        public int? SubscriptionPlanItemActionID { get; set; }
        public int? SubscriptionPlanItemID { get; set; }
        public int? SubscriptionActionTypeID { get; set; }
        public decimal? SubscriptionActionAmount { get; set; }
        public string SubscriptionActionProductCode { get; set; }
        public int? SubscriptionActionQuantity { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SubscriptionPlanItemID", SubscriptionPlanItemID);
            v.AssertNotNull("SubscriptionActionTypeID", SubscriptionActionTypeID);
            v.AssertString("SubscriptionActionProductCode", SubscriptionActionProductCode, 50);
        }
    }
}
