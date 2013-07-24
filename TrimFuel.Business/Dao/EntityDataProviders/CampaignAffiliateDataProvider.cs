using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignAffiliateDataProvider : EntityDataProvider<CampaignAffiliate>
    {
        private const string INSERT_COMMAND = "INSERT INTO CampaignAffiliate(CampaignID, AffiliateID) VALUES(@CampaignID, @AffiliateID);";
        private const string UPDATE_COMMAND = "UPDATE CampaignAffiliate SET CampaignID=@CampaignID, AffiliateID=@AffiliateID WHERE CampaignID=@IDCampaignID and AffiliateID=@IDAffiliateID;";
        private const string SELECT_COMMAND = "SELECT * FROM CampaignAffiliate WHERE CampaignID=@IDCampaignID and AffiliateID=@IDAffiliateID;";

        public override void Save(CampaignAffiliate entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CampaignAffiliateID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDCampaignID", MySqlDbType.Int32).Value = entity.CampaignAffiliateID.Value.CampaignID;
                cmd.Parameters.Add("@IDAffiliateID", MySqlDbType.Int32).Value = entity.CampaignAffiliateID.Value.AffiliateID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@AffiliateID", MySqlDbType.Int32).Value = entity.AffiliateID;

            cmd.ExecuteNonQuery();
            entity.CampaignAffiliateID = new CampaignAffiliate.ID() { CampaignID = entity.CampaignID.Value, AffiliateID = entity.AffiliateID.Value };
        }

        public override CampaignAffiliate Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDCampaignID", MySqlDbType.Int32).Value = ((CampaignAffiliate.ID?)key).Value.CampaignID;
            cmd.Parameters.Add("@IDAffiliateID", MySqlDbType.Int32).Value = ((CampaignAffiliate.ID?)key).Value.AffiliateID;

            return Load(cmd).FirstOrDefault();
        }

        public override CampaignAffiliate Load(DataRow row)
        {
            CampaignAffiliate res = new CampaignAffiliate();

            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["AffiliateID"] is DBNull))
                res.AffiliateID = Convert.ToInt32(row["AffiliateID"]);

            return res;
        }
    }
}
