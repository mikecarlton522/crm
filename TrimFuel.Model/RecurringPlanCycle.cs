using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RecurringPlanCycle : Entity
    {
        public int? RecurringPlanCycleID { get; set; }
        public int? RecurringPlanID { get; set; }
        public int? Interim { get; set; }
        public int? RetryInterim { get; set; }
        public int? Cycle { get; set; }
        public bool? Recurring { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("RecurringPlanID", RecurringPlanID);
            v.AssertNotNull("Interim", Interim);
            v.AssertNotNull("RetryInterim", RetryInterim);
            v.AssertNotNull("Cycle", Cycle);
            v.AssertNotNull("Recurring", Recurring);
        }
    }
}