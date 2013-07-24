using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingSale : Sale
    {
        public int? BillingSubscriptionID { get; set; }
        public long? ChargeHistoryID { get; set; }
        public long? PaygeaID { get; set; }
        public int? RebillCycle { get; set; }
        public string ProductCode { get; set; }
        public int? Quantity { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            base.ValidateFields(v);
            
            v.AssertNotNull("RebillCycle", RebillCycle);
            v.AssertString("ProductCode", ProductCode, 50);
        }

        #region Logic

        public void FillFromSale(Sale sale)
        {
            if (sale != null)
            {
                SaleID = sale.SaleID;
                SaleTypeID = sale.SaleTypeID;
                TrackingNumber = sale.TrackingNumber;
                CreateDT = sale.CreateDT;
                NotShip = sale.NotShip;
            }
        }

        #endregion
    }
}
