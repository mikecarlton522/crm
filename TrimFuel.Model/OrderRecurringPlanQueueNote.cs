using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderRecurringPlanQueueNote : Entity
    {
        public struct ID
        {
            public long OrderRecirringPlanID { get; set; }
        }

        public ID? OrderRecurringPlanQueueNoteID { get; set; }
        public long? OrderRecurringPlanID { get; set; }
        public string Reason { get; set; }
        public decimal? Amount { get; set; }
        public int? MerchantAccountID { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("OrderRecurringPlanID", OrderRecurringPlanID);
            v.AssertString("Reason", Reason, 255);
        }
    }
}
