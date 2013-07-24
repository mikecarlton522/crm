using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SalesAgrByReasonReportViewDataProvider : EntityViewDataProvider<SalesAgrByReasonReportView>
    {
        public override SalesAgrByReasonReportView Load(System.Data.DataRow row)
        {
            SalesAgrByReasonReportView res = new SalesAgrByReasonReportView();
            SalesAgrBaseReportView view = (new SalesAgrBaseReportViewDataProvider()).Load(row);
            view.Fill(res);

            if (!(row["ChargebackReasonCode"] is DBNull))
                res.ChargebackReasonCode = Convert.ToString(row["ChargebackReasonCode"]);
            if (!(row["ChargebackStatus"] is DBNull))
                res.ChargebackStatus = Convert.ToString(row["ChargebackStatus"]);
            if (!(row["ChargebackReasonCodeID"] is DBNull))
                res.ChargebackReasonCodeID = Convert.ToInt32(row["ChargebackReasonCodeID"]);
            if (!(row["ChargebackStatusTID"] is DBNull))
                res.ChargebackStatusTID = Convert.ToInt32(row["ChargebackStatusTID"]);

            return res;
        }
    }
}
