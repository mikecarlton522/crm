using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SaleShipperViewDataProvider : EntityViewDataProvider<SaleShipperView>
    {
        public override SaleShipperView Load(DataRow row)
        {
            SaleShipperView res = new SaleShipperView();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt32(row["SaleTypeID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
