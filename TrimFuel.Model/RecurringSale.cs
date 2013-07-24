using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RecurringSale : OrderSale
    {
        public long? OrderRecurringPlanID { get; set; }
        public int? RecurringCycle { get; set; }
        public bool? ReAttempt { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            base.ValidateFields(v);
            v.AssertNotNull("OrderRecurringPlanID", OrderRecurringPlanID);
            v.AssertNotNull("RecurringCycle", RecurringCycle);
            v.AssertNotNull("ReAttempt", ReAttempt);
        }
    }
}
