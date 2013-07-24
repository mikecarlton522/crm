using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ShippingEventViewDataProvider : EntityViewDataProvider<ShippingEventView>
    {
        public override ShippingEventView Load(DataRow row)
        {
            ShippingEventView res = new ShippingEventView();

            if (!(row["EventText"] is DBNull))
                res.EventText = Convert.ToString(row["EventText"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt16(row["ShipperID"]);
            if (!(row["ResultShipmentStatus"] is DBNull))
                res.ResultShipmentStatus = Convert.ToInt32(row["ResultShipmentStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
