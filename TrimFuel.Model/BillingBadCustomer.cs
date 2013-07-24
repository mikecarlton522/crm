using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingBadCustomer : Entity
    {
        public int? BillingBadCustomerID { get; set; }
        public long? BillingID { get; set; }
        public string TransactionId { get; set; }
        public byte? Error { get; set; }
        public byte? Found { get; set; }
        public string Result { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("BillingID", BillingID);
            v.AssertString("TransactionId", TransactionId, 50);
            v.AssertString("Result", Result, 100);
            v.AssertString("Request", Request, 2000);
            v.AssertString("Response", Response, 2000);
        }
    }
}
