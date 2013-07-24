using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class UnsentUnpayedViewDataProvider : EntityViewDataProvider<UnsentUnpayedView>
    {
        public override UnsentUnpayedView Load(System.Data.DataRow row)
        {
            UnsentUnpayedView res = new UnsentUnpayedView();

            if (!(row["ID"] is DBNull))
                res.ID = Convert.ToInt32(row["ID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["GroupID"] is DBNull))
                res.GroupID = Convert.ToInt32(row["GroupID"]);
            if (!(row["FirstName"] is DBNull))
                res.FirstName = Convert.ToString(row["FirstName"]);
            if (!(row["BillType"] is DBNull))
                res.BillType = Convert.ToString(row["BillType"]);
            if (!(row["LastName"] is DBNull))
                res.LastName = Convert.ToString(row["LastName"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["SKU"] is DBNull))
                res.SKU = Convert.ToString(row["SKU"]);

            return res;
        }
    }
}
