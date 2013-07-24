using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingBatchEmail : Entity
    {
        public long? BillingBatchEmailID { get; set; }
        public int? BillingBatchEmailTypeID { get; set; }
        public long? BillingID { get; set; }
        public long? EmailID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingBatchEmailTypeID", BillingBatchEmailTypeID);
            v.AssertNotNull("BillingID", BillingID);
            v.AssertNotNull("EmailID", EmailID);
        }
    }
}
