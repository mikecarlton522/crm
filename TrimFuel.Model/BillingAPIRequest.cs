using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingAPIRequest : Entity
    {
        public long? BillingAPIRequestID { get; set; }
        public DateTime? CreateDT { get; set; }
        public string IP { get; set; }
        public string URL { get; set; }
        public string Method { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertString("URL", URL, 255);
            v.AssertString("IP", IP, 100);
            v.AssertString("Method", Method, 100);
            v.AssertString("Request", Request, 4000);
            v.AssertString("Response", Response, 4000);
        }
    }
}
