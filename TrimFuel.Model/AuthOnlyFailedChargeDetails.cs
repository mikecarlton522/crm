using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AuthOnlyFailedChargeDetails : FailedChargeHistory
    {
        public decimal? RequestedAmount { get; set; }
        public int? RequestedCurrencyID { get; set; }
        public decimal? RequestedCurrencyAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            base.ValidateFields(v);

            v.AssertNotNull("RequestedAmount", RequestedAmount);
        }

        #region Logic

        public void FillFromChargeHistory(FailedChargeHistory failedChargeHistory)
        {
            if (failedChargeHistory != null)
            {
                FailedChargeHistoryID = failedChargeHistory.FailedChargeHistoryID;
                BillingID = failedChargeHistory.BillingID;
                ChargeDate = failedChargeHistory.ChargeDate;
                Amount = failedChargeHistory.Amount;
                Response = failedChargeHistory.Response;
                Success = failedChargeHistory.Success;
                SaleTypeID = failedChargeHistory.SaleTypeID;
                MerchantAccountID = failedChargeHistory.MerchantAccountID;
            }
        }

        #endregion
    }
}
