using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderRecurringPlan : Entity
    {
        public long? OrderRecurringPlanID { get; set; }
        public long? SaleID { get; set; }
        public int? RecurringPlanID { get; set; }
        public int? TrialInterim { get; set; }
        public int? RecurringStatus { get; set; }
        public int? DiscountTypeID { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? NextCycleDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("RecurringPlanID", RecurringPlanID);
            v.AssertNotNull("TrialInterim", TrialInterim);
            v.AssertNotNull("RecurringStatus", RecurringStatus);
        }
    }
}
