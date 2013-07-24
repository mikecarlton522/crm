using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ChargeDetails : ChargeHistoryEx
    {
        public int? SaleTypeID { get; set; }
        public string SKU { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("SKU", SKU, 255);
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
