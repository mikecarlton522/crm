using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ShortShipmentViewDataProvider : EntityViewDataProvider<ShortShipmentView>
    {
        public override ShortShipmentView Load(DataRow row)
        {
            ShortShipmentView res = new ShortShipmentView();
            if (!(row["RegID"] is DBNull))
                res.RegID = Convert.ToInt64(row["RegID"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            return res;
        }
    }
}
