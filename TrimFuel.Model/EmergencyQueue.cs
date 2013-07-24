using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class EmergencyQueue : Entity
    {
        public int? EmergencyQueueID { get; set; }
        public int? BillingID { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool? Completed { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
