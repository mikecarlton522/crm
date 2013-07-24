using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ExtraTrialShipSale : Sale
    {
        public int? ExtraTrialShipID { get; set; }
        public long? BillingID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            base.ValidateFields(v);

            v.AssertNotNull("ExtraTrialShip", ExtraTrialShipID);
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
