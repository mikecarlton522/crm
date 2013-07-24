using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ReturnsReportViewDataProvider : EntityViewDataProvider<ReturnsReportView>
    {
        public override ReturnsReportView Load(DataRow row)
        {
            ReturnsReportView res = new ReturnsReportView();

            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["FirstName"] is DBNull))
                res.FirstName = Convert.ToString(row["FirstName"]);
            if (!(row["LastName"] is DBNull))
                res.LastName = Convert.ToString(row["LastName"]);
            if (!(row["RefundCreateDT"] is DBNull))
                res.RefundCreateDT = Convert.ToDateTime(row["RefundCreateDT"]);
            if (!(row["RefundReason"] is DBNull))
                res.RefundReason = Convert.ToString(row["RefundReason"]);
            if (!(row["CallRMA"] is DBNull))
                res.CallRMA = Convert.ToString(row["CallRMA"]);
            if (!(row["DispositionDisplayName"] is DBNull))
                res.DispositionDisplayName = Convert.ToString(row["DispositionDisplayName"]);

            return res;
        }
    }
}
