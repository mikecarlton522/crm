using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingReferred : Entity
    {
        public int? BillingReferredID { get; set; }
        public int? BillingID { get; set; }
        public int? ReferralBillingID { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool? ExtraGiftCompleted { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
