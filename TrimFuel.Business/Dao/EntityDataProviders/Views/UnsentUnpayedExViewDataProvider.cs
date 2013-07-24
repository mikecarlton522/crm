using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class UnsentUnpayedExViewDataProvider : EntityViewDataProvider<UnsentUnpayedExView>
    {
        public override UnsentUnpayedExView Load(System.Data.DataRow row)
        {
            UnsentUnpayedExView res = new UnsentUnpayedExView();

            if (!(row["ID"] is DBNull))
                res.ID = Convert.ToInt32(row["ID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["FirstName"] is DBNull))
                res.FirstName = Convert.ToString(row["FirstName"]);
            if (!(row["BillType"] is DBNull))
                res.BillType = Convert.ToString(row["BillType"]);
            if (!(row["LastName"] is DBNull))
                res.LastName = Convert.ToString(row["LastName"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);
            if (!(row["CreditCard"] is DBNull))
                res.CreditCard = Convert.ToString(row["CreditCard"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);

            return res;
        }
    }
}
