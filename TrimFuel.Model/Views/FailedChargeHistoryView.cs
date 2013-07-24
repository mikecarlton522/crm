using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class FailedChargeHistoryView : EntityView
    {
        public FailedChargeHistoryView()
        {
        }

        public FailedChargeHistoryView(FailedChargeHistory src)
        {
            FailedChargeHistoryID = src.FailedChargeHistoryID;
            BillingID = src.BillingID;
            ChargeDate = src.ChargeDate;
            Amount = src.Amount;
            Response = src.Response;
            Success = src.Success;
            SaleTypeID = src.SaleTypeID;
            MerchantAccountID = src.MerchantAccountID;
        }

        public int? FailedChargeHistoryID { get; set; }
        public long? BillingID { get; set; }
        public DateTime? ChargeDate { get; set; }
        public decimal? Amount { get; set; }
        public string Response { get; set; }
        public bool? Success { get; set; }
        public int? SaleTypeID { get; set; }
        public int? MerchantAccountID { get; set; }
        public string ChildMID { get; set; }
    }
}
