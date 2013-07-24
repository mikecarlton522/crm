using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SubscriptionPlanItemActionView : EntityView
    {
        public int? SubscriptionPlanItemActionID { get; set; }
        public int? SubscriptionPlanItemID { get; set; }
        public int? SubscriptionActionTypeID { get; set; }
        public string SubscriptionActionTypeName { get; set; }
        public decimal? SubscriptionActionAmount { get; set; }
        public string SubscriptionActionProductCode { get; set; }
        public string SubscriptionActionProductName { get; set; }
        public int? SubscriptionActionQuantity { get; set; }
    }
}
