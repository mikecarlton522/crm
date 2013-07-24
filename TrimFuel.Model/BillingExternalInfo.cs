using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingExternalInfo : Entity
    {
        public long? BillingID { get; set; }
        public string InternalID { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            //Billing 1 <-> 0..1 BillingExternalInfo association
            v.AssertNotNull("BillingID", BillingID);
            v.AssertString("CustomField1", CustomField1, 255);
            v.AssertString("CustomField2", CustomField2, 255);
            v.AssertString("CustomField3", CustomField3, 255);
            v.AssertString("CustomField4", CustomField4, 255);
            v.AssertString("CustomField5", CustomField5, 255);
        }
    }
}
