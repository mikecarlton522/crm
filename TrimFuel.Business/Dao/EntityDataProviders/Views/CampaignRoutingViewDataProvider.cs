using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class CampaignRoutingViewDataProvider : EntityViewDataProvider<CampaignRoutingView>
    {
        public override CampaignRoutingView Load(DataRow row)
        {
            CampaignRoutingView res = new CampaignRoutingView();

            if (!(row["URL"] is DBNull))
                res.URL = Convert.ToString(row["URL"]);
            if (!(row["ExtUrl"] is DBNull))
                res.ExtUrl = Convert.ToString(row["ExtUrl"]);
            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["SubAffiliate"] is DBNull))
                res.SubAffiliate = Convert.ToString(row["SubAffiliate"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToDecimal(row["Percentage"]);

            return res;
        }
    }
}
