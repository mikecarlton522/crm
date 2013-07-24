using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingBatchEmailType : Entity
    {
        public int? BillingBatchEmailTypeID { get; set; }
        public string Name { get; set; }
        public int? DynamicEmailID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("DynamicEmailID", DynamicEmailID);
            v.AssertNotNull("Name", Name);
            v.AssertString("Name", Name, 50);
        }
    }
}
