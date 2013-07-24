using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class InventorySaleViewDataProvider : EntityViewDataProvider<InventorySaleView>
    {
        public override InventorySaleView Load(DataRow row)
        {
            InventorySaleView res = new InventorySaleView();

            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);
            if (!(row["InventoryProduct"] is DBNull))
                res.InventoryProduct = Convert.ToString(row["InventoryProduct"]);
            if (!(row["InventoryQuantity"] is DBNull))
                res.InventoryQuantity = Convert.ToInt32(row["InventoryQuantity"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["SaleCreateDT"] is DBNull))
                res.SaleCreateDT = Convert.ToDateTime(row["SaleCreateDT"]);
            if (!(row["SaleChargeAmount"] is DBNull))
                res.SaleChargeAmount = Convert.ToDecimal(row["SaleChargeAmount"]);
            if (!(row["SaleShippedDT"] is DBNull))
                res.SaleShippedDT = Convert.ToDateTime(row["SaleShippedDT"]);
            if (!(row["SaleTrackingNumber"] is DBNull))
                res.SaleTrackingNumber = Convert.ToString(row["SaleTrackingNumber"]);
            if (!(row["SaleReturnDT"] is DBNull))
                res.SaleReturnDT = Convert.ToDateTime(row["SaleReturnDT"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["BillingFirstName"] is DBNull))
                res.BillingFirstName = Convert.ToString(row["BillingFirstName"]);
            if (!(row["BillingLastName"] is DBNull))
                res.BillingLastName = Convert.ToString(row["BillingLastName"]);

            return res;
        }
    }
}
