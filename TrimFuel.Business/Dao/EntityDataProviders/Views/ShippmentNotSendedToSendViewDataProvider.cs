using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ShippmentNotSendedToSendViewDataProvider : EntityViewDataProvider<ShippmentNotSendedToSendView>
    {
        public override ShippmentNotSendedToSendView Load(System.Data.DataRow row)
        {
            ShippmentNotSendedToSendView res = new ShippmentNotSendedToSendView();
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt32(row["SaleID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            return res;
        }
    }
}
