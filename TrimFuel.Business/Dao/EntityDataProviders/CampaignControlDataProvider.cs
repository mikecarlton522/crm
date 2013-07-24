using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignControlDataProvider : EntityDataProvider<CampaignControl>
    {
        public override void Save(CampaignControl entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override CampaignControl Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override CampaignControl Load(DataRow row)
        {
            CampaignControl res = new CampaignControl();

            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["HTML"] is DBNull))
                res.HTML = Convert.ToString(row["HTML"]);           

            return res;
        }
    }
}
