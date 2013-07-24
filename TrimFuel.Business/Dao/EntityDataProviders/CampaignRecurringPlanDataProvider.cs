using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignRecurringPlanDataProvider : EntityDataProvider<CampaignRecurringPlan>
    {
        private const string INSERT_COMMAND = "INSERT INTO CampaignRecurringPlan(CampaignID, RecurringPlanID, TrialPrice, TrialInterim) VALUES(@CampaignID, @RecurringPlanID, @TrialPrice, @TrialInterim); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE CampaignRecurringPlan SET CampaignID=@CampaignID, RecurringPlanID=@RecurringPlanID, TrialPrice=@TrialPrice, TrialInterim=@TrialInterim WHERE CampaignRecurringPlanID=@CampaignRecurringPlanID;";
        private const string SELECT_COMMAND = "SELECT * FROM CampaignRecurringPlan WHERE CampaignRecurringPlanID=@CampaignRecurringPlanID;";

        public override void Save(CampaignRecurringPlan entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CampaignRecurringPlanID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CampaignRecurringPlanID", MySqlDbType.Int32).Value = entity.CampaignRecurringPlanID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@RecurringPlanID", MySqlDbType.Int32).Value = entity.RecurringPlanID;
            cmd.Parameters.Add("@TrialPrice", MySqlDbType.Decimal).Value = entity.TrialPrice;
            cmd.Parameters.Add("@TrialInterim", MySqlDbType.Int32).Value = entity.TrialInterim;


            if (entity.CampaignRecurringPlanID == null)
            {
                entity.CampaignRecurringPlanID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("CampaignRecurringPlan({0}) was not found in database.", entity.CampaignRecurringPlanID));
                }
            }
        }

        public override CampaignRecurringPlan Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignRecurringPlanID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override CampaignRecurringPlan Load(DataRow row)
        {
            CampaignRecurringPlan res = new CampaignRecurringPlan();

            if (!(row["CampaignRecurringPlanID"] is DBNull))
                res.CampaignRecurringPlanID = Convert.ToInt32(row["CampaignRecurringPlanID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["RecurringPlanID"] is DBNull))
                res.RecurringPlanID = Convert.ToInt32(row["RecurringPlanID"]);
            if (!(row["TrialPrice"] is DBNull))
                res.TrialPrice = Convert.ToDecimal(row["TrialPrice"]);
            if (!(row["TrialInterim"] is DBNull))
                res.TrialInterim = Convert.ToInt32(row["TrialInterim"]);

            return res;
        }
    }
}
