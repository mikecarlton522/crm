using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ProspectViewDataProvider : EntityViewDataProvider<ProspectView>
    {
        public override ProspectView Load(System.Data.DataRow row)
        {
            ProspectView res = new ProspectView();

            if (!(row["CampaignName"] is DBNull))
                res.CampaignName = Convert.ToString(row["CampaignName"]);
            res.Registration = EntityDataProvider<Registration>.CreateProvider().Load(row);
            //res.RegistrationInfo = EntityDataProvider<RegistrationInfo>.CreateProvider().Load(row);

            return res;
        }
    }
}
