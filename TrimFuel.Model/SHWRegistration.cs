using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SHWRegistration : Entity
    {
        public int? SHWRegistrationID { get; set; }
        public long? BillingID { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string SHWID { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingID", BillingID);
            v.AssertNotNull("Request", Request);
            v.AssertNotNull("Response", Response);
            v.AssertString("SHWID", SHWID, 1024);
        }
    }
}
