using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Email : Entity
    {
        public long? EmailID { get; set; }
        public int? DynamicEmailID { get; set; }
        public long? BillingID { get; set; }
        public string Email_ { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Response { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("DynamicEmailID", DynamicEmailID);
            v.AssertString("Email", Email_, 255);
            v.AssertString("Subject", Subject, 255);
        }
    }
}
