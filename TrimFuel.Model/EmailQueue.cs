using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class EmailQueue : Entity
    {
        public int? EmailQueueID { get; set; }
        public long? BillingID { get; set; }
        public long? SaleID { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool? Completed { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
