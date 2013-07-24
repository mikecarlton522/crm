using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SubscriptionPlanItemActionViewDataProvider : EntityViewDataProvider<SubscriptionPlanItemActionView>
    {
        public override SubscriptionPlanItemActionView Load(System.Data.DataRow row)
        {
            SubscriptionPlanItemActionView res = new SubscriptionPlanItemActionView();

            if (!(row["SubscriptionPlanItemActionID"] is DBNull))
                res.SubscriptionPlanItemActionID = Convert.ToInt32(row["SubscriptionPlanItemActionID"]);
            if (!(row["SubscriptionPlanItemID"] is DBNull))
                res.SubscriptionPlanItemID = Convert.ToInt32(row["SubscriptionPlanItemID"]);
            if (!(row["SubscriptionActionTypeID"] is DBNull))
                res.SubscriptionActionTypeID = Convert.ToInt32(row["SubscriptionActionTypeID"]);
            if (!(row["SubscriptionActionTypeName"] is DBNull))
                res.SubscriptionActionTypeName = Convert.ToString(row["SubscriptionActionTypeName"]);
            if (!(row["SubscriptionActionAmount"] is DBNull))
                res.SubscriptionActionAmount = Convert.ToDecimal(row["SubscriptionActionAmount"]);
            if (!(row["SubscriptionActionProductCode"] is DBNull))
                res.SubscriptionActionProductCode = Convert.ToString(row["SubscriptionActionProductCode"]);
            if (!(row["SubscriptionActionProductName"] is DBNull))
                res.SubscriptionActionProductName = Convert.ToString(row["SubscriptionActionProductName"]);
            if (!(row["SubscriptionActionQuantity"] is DBNull))
                res.SubscriptionActionQuantity = Convert.ToInt32(row["SubscriptionActionQuantity"]);

            return res;
        }
    }
}
