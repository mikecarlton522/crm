using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class InventoryViewDataProvider : EntityViewDataProvider<InventoryView>
    {
        public override InventoryView Load(DataRow row)
        {
            InventoryView res = new InventoryView();

            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);
            if (!(row["SKU"] is DBNull))
                res.SKU = Convert.ToString(row["SKU"]);
            if (!(row["Product"] is DBNull))
                res.Product = Convert.ToString(row["Product"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
