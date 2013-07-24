using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class CustomShipperToSendViewDataProvider : EntityViewDataProvider<CustomShipperToSendView>
    {
        public override CustomShipperToSendView Load(DataRow row)
        {
            var res = new CustomShipperToSendView();

            if (!(row["BillAmount"] is DBNull))
                res.BillAmount = Convert.ToDecimal(row["BillAmount"]);
            if (!(row["PartNumber"] is DBNull))
                res.PartNumber = Convert.ToString(row["PartNumber"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["UnitWeight"] is DBNull))
                res.UnitWeight = Convert.ToDecimal(row["UnitWeight"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);

            return res;
        }
    }
}
