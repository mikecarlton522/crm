using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class LeadRoutingViewDataProvider : EntityViewDataProvider<LeadRoutingView>
    {
        public override LeadRoutingView Load(DataRow row)
        {
            LeadRoutingView res = new LeadRoutingView();

            res.LeadRouting = EntityDataProvider<LeadRouting>.CreateProvider().Load(row);

            if (!(row["ProductName"] is DBNull))
                res.ProductName = Convert.ToString(row["ProductName"]);
            if (!(row["LeadPartnerName"] is DBNull))
                res.LeadPartnerName = Convert.ToString(row["LeadPartnerName"]);            

            return res;
        }
    }
}
