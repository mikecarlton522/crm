using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class OrderSaleBillingViewDataProvider : EntityViewDataProvider<OrderSaleBillingView>
    {
        public override OrderSaleBillingView Load(DataRow row)
        {
            OrderSaleBillingView res = new OrderSaleBillingView();

            res.Sale = EntityDataProvider<OrderSale>.CreateProvider().Load(row);

            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            
            return res;
        }
    }
}
