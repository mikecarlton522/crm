using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class RefererViewDataProvider : EntityViewDataProvider<RefererView>
    {
        public override RefererView Load(DataRow row)
        {
            RefererView res = new RefererView();

            res.Referer = (new RefererDataProvider()).Load(row);

            if (!(row["SalesCount"] is DBNull))
                res.SalesCount = Convert.ToInt32(row["SalesCount"]);
            if (!(row["SalesAmount"] is DBNull))
                res.SalesAmount = Convert.ToDecimal(row["SalesAmount"]);

            return res;
        }
    }
}
