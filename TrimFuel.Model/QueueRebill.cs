using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class QueueRebill : Entity
    {
        public int? QueueRebillID { get; set; }
        public int? BillingSubscriptionID { get; set; }
        public string Reason { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {

        }
    }
}
