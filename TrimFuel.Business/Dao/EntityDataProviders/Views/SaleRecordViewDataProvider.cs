using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SaleRecordViewDataProvider : EntityViewDataProvider<SaleRecordView>
    {
        public override SaleRecordView Load(System.Data.DataRow row)
        {
            SaleRecordView res = new SaleRecordView();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["TrackingNumber"] is DBNull))
                res.TrackingNumber = Convert.ToString(row["TrackingNumber"]);
            if (!(row["ShipmentMethod"] is DBNull))
                res.ShipmentMethod = Convert.ToString(row["ShipmentMethod"]);
            
            return res;
        }
    }
}
