using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class WebStoreDataProvider : EntityDataProvider<WebStore>
    {
        //private const string INSERT_COMMAND = "INSERT INTO WebStore VALUES(@CampaignID);";
        //private const string UPDATE_COMMAND = "UPDATE BillingSale SET CampaignID=@CampaignID WHERE CampaignID=@CampaignID;";
        private const string SELECT_COMMAND = "SELECT * FROM Campaign c INNER JOIN WebStore ws ON ws.CampaignID = c.CampaignID WHERE c.CampaignID = @CampaignID;";

        public override void Save(WebStore entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override WebStore Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override WebStore Load(System.Data.DataRow row)
        {
            Campaign campaign = (new CampaignDataProvider()).Load(row);

            WebStore res = new WebStore();
            res.FillFromCampaign(campaign);

            return res;
        }
    }
}
