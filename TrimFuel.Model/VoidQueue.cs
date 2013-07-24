using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class VoidQueue : Entity
    {
        public int? VoidQueueID { get; set; }
        public long? BillingID { get; set; }
        public long? SaleID { get; set; }
        public decimal? Amount { get; set; }
        public bool? Completed { get; set; }
        public DateTime? SaleChargeDT { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingID", BillingID);
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("Amount", Amount);
            v.AssertNotNull("Completed", Completed);
        }
    }
}
