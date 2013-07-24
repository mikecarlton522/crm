using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class FailedChargeHistory : Entity
    {
        public int? FailedChargeHistoryID { get; set; }
        public long? BillingID { get; set; }
        public DateTime? ChargeDate { get; set; }
        public decimal? Amount { get; set; }
        public string Response { get; set; }
        public bool? Success { get; set; }
        public int? SaleTypeID { get; set; }
        public int? MerchantAccountID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Response", Response, 2024);
        }
    }
}
