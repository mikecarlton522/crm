using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingStopCharge : Entity
    {
        public long? BillingStopChargeID { get; set; }
        public long? BillingID { get; set; }
        public string StopReason { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingID", BillingID);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertString("StopReason", StopReason, 255);
        }
    }
}
