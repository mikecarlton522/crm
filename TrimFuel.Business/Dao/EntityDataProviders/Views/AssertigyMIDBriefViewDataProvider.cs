using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class AssertigyMIDBriefViewDataProvider : EntityViewDataProvider<AssertigyMIDBriefView>
    {
        public override AssertigyMIDBriefView Load(DataRow row)
        {
            AssertigyMIDBriefView res = new AssertigyMIDBriefView();

            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["MID"] is DBNull))
                res.MID = Convert.ToString(row["MID"]);

            return res;
        }
    }
}
