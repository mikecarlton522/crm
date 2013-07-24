using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SalesAgrByAffReportViewDataProvider : EntityViewDataProvider<SalesAgrByAffReportView>
    {
        public override SalesAgrByAffReportView Load(DataRow row)
        {
            SalesAgrByAffReportView res = new SalesAgrByAffReportView();
            SalesAgrBaseReportView view = (new SalesAgrBaseReportViewDataProvider()).Load(row);
            view.Fill(res);

            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["SubAffiliate"] is DBNull))
                res.SubAffiliate = Convert.ToString(row["SubAffiliate"]);

            return res;
        }
    }
}
