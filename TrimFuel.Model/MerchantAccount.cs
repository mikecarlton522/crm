using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class MerchantAccount : Entity
    {
        public int? MerchantAccountID { get; set; }
        public string Provider { get; set; }
        public string FriendlyName { get; set; }
        public string AccountNumber { get; set; }
        public string MerchantID { get; set; }
        public string MerchantPassword { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool? Active { get; set; }
        public decimal? DailyCap { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Provider", Provider, 50);
            v.AssertString("FriendlyName", FriendlyName, 50);
            v.AssertString("AccountNumber", AccountNumber, 50);
            v.AssertString("MerchantID", MerchantID, 50);
            v.AssertString("MerchantPassword", MerchantPassword, 50);
            v.AssertNotNull("Active", Active);
            v.AssertNotNull("DailyCap", DailyCap);
        }
    }
}
