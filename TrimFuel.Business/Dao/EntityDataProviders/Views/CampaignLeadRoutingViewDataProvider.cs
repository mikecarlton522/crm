using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class CampaignLeadRoutingViewDataProvider : EntityViewDataProvider<CampaignLeadRoutingView>
    {
        public override CampaignLeadRoutingView Load(DataRow row)
        {
            CampaignLeadRoutingView res = new CampaignLeadRoutingView();

            res.CampaignLeadRouting = EntityDataProvider<CampaignLeadRouting>.CreateProvider().Load(row);

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["ProductName"] is DBNull))
                res.ProductName = Convert.ToString(row["ProductName"]);
            if (!(row["LeadPartnerName"] is DBNull))
                res.LeadPartnerName = Convert.ToString(row["LeadPartnerName"]);            

            return res;
        }
    }
}
