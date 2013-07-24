using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SalesAgrReportViewDataProvider : EntityViewDataProvider<SalesAgrReportView>
    {
        public override SalesAgrReportView Load(DataRow row)
        {
            SalesAgrReportView res = new SalesAgrReportView();
            SalesAgrBaseReportView view = (new SalesAgrBaseReportViewDataProvider()).Load(row);
            view.Fill(res);

            if (!(row["NMICompanyName"] is DBNull))
                res.NMICompanyName = Convert.ToString(row["NMICompanyName"]);

            return res;
        }
    }
}
