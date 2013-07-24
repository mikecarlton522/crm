using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class BillingCallViewDataProvider : EntityViewDataProvider<BillingCallView>
    {
        public override BillingCallView Load(DataRow row)
        {
            BillingCallView res = new BillingCallView();

            res.Billing = EntityDataProvider<Billing>.CreateProvider().Load(row);
            res.LastCall = EntityDataProvider<Call>.CreateProvider().Load(row);

            if (!(row["NumberOfCalls"] is DBNull))
                res.NumberOfCalls = Convert.ToInt32(row["NumberOfCalls"]);
            if (!(row["LastCallDate"] is DBNull))
                res.LastCall.CreateDT = Convert.ToDateTime(row["LastCallDate"]);
            if (!(row["BillingCreateDT"] is DBNull))
                res.Billing.CreateDT = Convert.ToDateTime(row["BillingCreateDT"]);

            return res;
        }
    }
}
