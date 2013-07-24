using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class GroupProductSKUViewDataProvider : EntityViewDataProvider<GroupProductSKU>
    {
        public override GroupProductSKU Load(DataRow row)
        {
            GroupProductSKU res = new GroupProductSKU();

            if (!(row["GroupProductSKU"] is DBNull))
                res.GroupProductSKU_ = Convert.ToString(row["GroupProductSKU"]);
            if (!(row["GroupProductName"] is DBNull))
                res.GroupProductName = Convert.ToString(row["GroupProductName"]);
            if (!(row["GroupVolume"] is DBNull))
                res.GroupVolume = Convert.ToInt32(row["GroupVolume"]);

            return res;
        }
    }
}
