using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SubscriptionPlanItem : Entity
    {
        public int? SubscriptionPlanItemID { get; set; }
        public int? NextSubscriptionPlanItemID { get; set; }
        public int? Interim { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Interim", Interim);
        }
    }
}
