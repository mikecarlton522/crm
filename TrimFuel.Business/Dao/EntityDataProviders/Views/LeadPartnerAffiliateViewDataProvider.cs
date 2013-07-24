using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class LeadPartnerAffiliateViewDataProvider : EntityViewDataProvider<LeadPartnerAffiliateView>
    {
        public override LeadPartnerAffiliateView Load(DataRow row)
        {
            LeadPartnerAffiliateView res = new LeadPartnerAffiliateView();

            res.LeadPartner = (new LeadPartnerDataProvider()).Load(row);
            res.Affiliate = (new AffiliateDataProvider()).Load(row);

            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["AffiliateID"] is DBNull))
                res.AffiliateID = Convert.ToInt32(row["AffiliateID"]);

            return res;
        }
    }
}
