using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class AssertigyMIDCapView2DataProvider : EntityViewDataProvider<AssertigyMIDCapView2>
    {
        public override AssertigyMIDCapView2 Load(DataRow row)
        {
            AssertigyMIDCapView2 res = new AssertigyMIDCapView2();

            if (!(row["RemainingCap"] is DBNull))
                res.RemainingCap = Convert.ToDecimal(row["RemainingCap"]);
            res.MID = EntityDataProvider<AssertigyMID>.CreateProvider().Load(row);

            return res;
        }
    }
}
