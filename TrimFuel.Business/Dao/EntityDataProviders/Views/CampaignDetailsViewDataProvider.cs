using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class CampaignDetailsViewDataProvider : EntityViewDataProvider<CampaignDetailsView>
    {
        public override CampaignDetailsView Load(DataRow row)
        {
            CampaignDetailsView res = new CampaignDetailsView();

            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["Corporation"] is DBNull))
                res.Corporation = Convert.ToString(row["Corporation"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["Email"] is DBNull))
                res.Email = Convert.ToString(row["Email"]);
            if (!(row["Phone"] is DBNull))
                res.Phone = Convert.ToString(row["Phone"]);
            if(!(row["RegistrationCount"] is DBNull))
                res.RegistrationCount = Convert.ToInt32(row["RegistrationCount"]);
            if (!(row["URL"] is DBNull))
                res.URL = Convert.ToString(row["URL"]);
            if (!(row["IsMerchant"] is DBNull))
                res.IsMerchant = Convert.ToBoolean(row["IsMerchant"]);
            if (!(row["IsExternal"] is DBNull))
                res.IsExternal = Convert.ToBoolean(row["IsExternal"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);

            return res;
        }
    }
}
