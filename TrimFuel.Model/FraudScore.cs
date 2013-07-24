using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class FraudScore : Entity
    {
        public int? FraudScoreID { get; set; }
        public long? SaleID { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public bool? Error { get; set; }
        public short? FraudScore_ { get; set; }
        public DateTime? CreateDT { get; set; }
        public long? BillingID { get; set; }
        public string ResponseBinName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Request", Request, 2000);
            v.AssertString("Response", Response, 2000);
            v.AssertString("ResponseBinName", ResponseBinName, 100);
        }
    }
}
