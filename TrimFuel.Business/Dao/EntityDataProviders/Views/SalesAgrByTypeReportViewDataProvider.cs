using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class SalesAgrByTypeReportViewDataProvider : EntityViewDataProvider<SalesAgrByTypeReportView>
    {
        public override SalesAgrByTypeReportView Load(DataRow row)
        {
            SalesAgrByTypeReportView res = new SalesAgrByTypeReportView();
            SalesAgrBaseReportView view = (new SalesAgrBaseReportViewDataProvider()).Load(row);
            view.Fill(res);

            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt32(row["SaleTypeID"]);

            return res;
        }
    }
}
