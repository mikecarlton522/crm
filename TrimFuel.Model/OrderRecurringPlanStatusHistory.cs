using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderRecurringPlanStatusHistory : Entity
    {
        public long? OrderRecurringPlanStatusHistoryID { get; set; }
        public long? OrderRecurringPlanID { get; set; }
        public int? RecurringStatus { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("OrderRecurringPlanID", OrderRecurringPlanID);
            v.AssertNotNull("RecurringStatus", RecurringStatus);
            v.AssertNotNull("CreateDT", CreateDT);
        }
    }
}
