using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class AssertigyMIDCapViewDataProvider : EntityViewDataProvider<AssertigyMIDCapView>
    {
        public override AssertigyMIDCapView Load(DataRow row)
        {
            AssertigyMIDCapView res = new AssertigyMIDCapView();

            if (!(row["RemainingCap"] is DBNull))
                res.RemainingCap = Convert.ToInt64(row["RemainingCap"]);
            res.AssertigyMID = EntityDataProvider<AssertigyMID>.CreateProvider().Load(row);

            return res;
        }
    }
}
