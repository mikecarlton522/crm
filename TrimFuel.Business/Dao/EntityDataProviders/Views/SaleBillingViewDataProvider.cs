using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SaleBillingViewDataProvider : EntityViewDataProvider<SaleBillingView>
    {
        public override SaleBillingView Load(DataRow row)
        {
            SaleBillingView res = new SaleBillingView();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["CreateDT"] is DBNull))
                res.SaleDT = Convert.ToDateTime(row["SaleDT"]);
            if (!(row["TotalAmount"] is DBNull))
                res.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
            if (!(row["ShippingAmount"] is DBNull))
                res.ShippingAmount = Convert.ToDecimal(row["ShippingAmount"]);

            res.Billing = (new BillingDataProvider()).Load(row);

            return res;
        }
    }
}
