using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class IgnoreUnbilledTransaction : Entity
    {
        public int? IgnoreUnbilledTransactionID { get; set; }
        public long? BillingID { get; set; }
        public int? BillingSubscriptionID { get; set; }
        public long? OrderRecurringPlanID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            
        }
    }
}
