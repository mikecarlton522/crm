using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SalesAgrBaseReportViewDataProvider : EntityViewDataProvider<SalesAgrBaseReportView>
    {
        public override SalesAgrBaseReportView Load(DataRow row)
        {
            SalesAgrBaseReportView res = new SalesAgrBaseReportView();

            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["AssertigyDisplayName"] is DBNull))
                res.AssertigyDisplayName = Convert.ToString(row["AssertigyDisplayName"]);
            if (!(row["AssertigyMID"] is DBNull))
                res.AssertigyMID = Convert.ToString(row["AssertigyMID"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);
            if (!(row["SaleCount"] is DBNull))
                res.SaleCount = Convert.ToInt32(row["SaleCount"]);

            return res;
        }
    }
}
