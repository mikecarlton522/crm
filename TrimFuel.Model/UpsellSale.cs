using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class UpsellSale : Sale
    {
        public int? UpsellID { get; set; }
        public long? ChargeHistoryID { get; set; }
        public long? PaygeaID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            base.ValidateFields(v);

            v.AssertNotNull("UpsellID", UpsellID);
        }

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
    }
}
