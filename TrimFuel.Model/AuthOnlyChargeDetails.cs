using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AuthOnlyChargeDetails : ChargeHistoryEx
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

        public void FillFromChargeHistory(ChargeHistoryEx chargeHistory)
        {
            if (chargeHistory != null)
            {
                ChargeHistoryID = chargeHistory.ChargeHistoryID;
                ChargeTypeID = chargeHistory.ChargeTypeID;
                MerchantAccountID = chargeHistory.MerchantAccountID;
                BillingSubscriptionID = chargeHistory.BillingSubscriptionID;
                ChargeDate = chargeHistory.ChargeDate;
                Amount = chargeHistory.Amount;
                AuthorizationCode = chargeHistory.AuthorizationCode;
                TransactionNumber = chargeHistory.TransactionNumber;
                Response = chargeHistory.Response;
                Success = chargeHistory.Success;
                ChildMID = chargeHistory.ChildMID;
            }
        }

        #endregion
    }
}
