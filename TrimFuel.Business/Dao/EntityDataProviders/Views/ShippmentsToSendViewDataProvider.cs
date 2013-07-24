using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ShippmentsToSendViewDataProvider : EntityViewDataProvider<ShippmentsToSendView>
    {
        public override ShippmentsToSendView Load(System.Data.DataRow row)
        {
            ShippmentsToSendView res = new ShippmentsToSendView();
            if (!(row["Value"] is DBNull))
                res.Count = Convert.ToInt32(row["Value"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            return res;
        }
    }
}
