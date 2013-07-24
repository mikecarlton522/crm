using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class PagePixelViewDataProvider : EntityViewDataProvider<PagePixelView>
    {
        public override PagePixelView Load(DataRow row)
        {
            PagePixelView res = new PagePixelView();

            if (!(row["PageID"] is DBNull))
                res.PageID = Convert.ToInt32(row["PageID"]);
            if (!(row["PixelCode"] is DBNull))
                res.PixelCode = Convert.ToString(row["PixelCode"]);

            return res;
        }
    }
}
