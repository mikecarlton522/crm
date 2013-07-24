using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignLeadRoutingDataProvider : EntityDataProvider<CampaignLeadRouting>
    {
        private const string INSERT_COMMAND = "INSERT INTO CampaignLeadRouting(CampaignID, LeadTypeID, LeadPartnerID, Percentage) VALUES(@CampaignID, @LeadTypeID, @LeadPartnerID, @Percentage);";
        private const string UPDATE_COMMAND = "UPDATE CampaignLeadRouting SET CampaignID=@CampaignID, LeadTypeID=@LeadTypeID, LeadPartnerID=@LeadPartnerID, Percentage=@Percentage WHERE CampaignID=@IDCampaignID and LeadTypeID=@IDLeadTypeID and LeadPartnerID=@IDLeadPartnerID;";
        private const string SELECT_COMMAND = "SELECT * FROM CampaignLeadRouting WHERE CampaignID=@IDCampaignID and LeadTypeID=@IDLeadTypeID and LeadPartnerID=@IDLeadPartnerID;";


        public override void Save(CampaignLeadRouting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CampaignLeadRoutingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDCampaignID", MySqlDbType.Int32).Value = entity.CampaignLeadRoutingID.Value.CampaignID;
                cmd.Parameters.Add("@IDLeadTypeID", MySqlDbType.Int32).Value = entity.CampaignLeadRoutingID.Value.LeadTypeID;
                cmd.Parameters.Add("@IDLeadPartnerID", MySqlDbType.Int32).Value = entity.CampaignLeadRoutingID.Value.LeadPartnerID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = entity.LeadTypeID;
            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = entity.LeadPartnerID;
            cmd.Parameters.Add("@Percentage", MySqlDbType.Int32).Value = entity.Percentage;

            cmd.ExecuteNonQuery();
            entity.CampaignLeadRoutingID = new CampaignLeadRouting.ID() { CampaignID = entity.CampaignID.Value, LeadTypeID = entity.LeadTypeID.Value, LeadPartnerID = entity.LeadPartnerID.Value };
        }

        public override CampaignLeadRouting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDCampaignID", MySqlDbType.Int32).Value = ((CampaignLeadRouting.ID?)key).Value.CampaignID;
            cmd.Parameters.Add("@IDLeadTypeID", MySqlDbType.Int32).Value = ((CampaignLeadRouting.ID?)key).Value.LeadTypeID;
            cmd.Parameters.Add("@IDLeadPartnerID", MySqlDbType.Int32).Value = ((CampaignLeadRouting.ID?)key).Value.LeadPartnerID;

            return Load(cmd).FirstOrDefault();
        }

        public override CampaignLeadRouting Load(DataRow row)
        {
            CampaignLeadRouting res = new CampaignLeadRouting();

            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["LeadTypeID"] is DBNull))
                res.LeadTypeID = Convert.ToInt32(row["LeadTypeID"]);
            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt32(row["Percentage"]);

            return res;
        }
    }
}
