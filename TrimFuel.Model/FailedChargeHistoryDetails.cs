using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class FailedChargeHistoryDetails : FailedChargeHistory
    {
        public int? SubscriptionID { get; set; }
        public string SKU { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("SKU", SKU, 255);
        }

        #region Logic

        public void FillFromChargeHistory(FailedChargeHistory failedChargeHistory)
        {
            if (failedChargeHistory != null)
            {
                FailedChargeHistoryID = failedChargeHistory.FailedChargeHistoryID;
                MerchantAccountID = failedChargeHistory.MerchantAccountID;
                ChargeDate = failedChargeHistory.ChargeDate;
                Amount = failedChargeHistory.Amount;
                Response = failedChargeHistory.Response;
                Success = failedChargeHistory.Success;
                BillingID = failedChargeHistory.BillingID;
            }
        }

        #endregion
    }
}
