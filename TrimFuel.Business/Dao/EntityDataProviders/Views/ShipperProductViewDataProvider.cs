using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ShipperProductViewDataProvider : EntityViewDataProvider<ShipperProductView>
    {
        public override ShipperProductView Load(System.Data.DataRow row)
        {
            ShipperProductView res = new ShipperProductView();

            if (!(row["Shipper"] is DBNull))
                res.Shipper = Convert.ToString(row["Shipper"]);
            if (!(row["Product"] is DBNull))
                res.Product = Convert.ToString(row["Product"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            
            return res;
        }
    }
}
