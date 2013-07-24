using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SubscriptionRecurringPlanViewDataProvider : EntityViewDataProvider<SubscriptionRecurringPlanView>
    {
        public override SubscriptionRecurringPlanView Load(System.Data.DataRow row)
        {
            SubscriptionRecurringPlanView res = new SubscriptionRecurringPlanView();

            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["RecurringPlanID"] is DBNull))
                res.RecurringPlanID = Convert.ToInt32(row["RecurringPlanID"]);

            return res;
        }
    }
}
