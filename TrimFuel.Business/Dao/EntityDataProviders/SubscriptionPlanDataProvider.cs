using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SubscriptionPlanDataProvider : EntityDataProvider<SubscriptionPlan>
    {
        private const string INSERT_COMMAND = "INSERT INTO SubscriptionPlan(SubscriptionPlanName, StartSubscriptionPlanItemID) VALUES(@SubscriptionPlanName, @StartSubscriptionPlanItemID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SubscriptionPlan SET SubscriptionPlanName=@SubscriptionPlanName, StartSubscriptionPlanItemID=@StartSubscriptionPlanItemID WHERE SubscriptionPlanID=@SubscriptionPlanID;";
        private const string SELECT_COMMAND = "SELECT * FROM SubscriptionPlan WHERE SubscriptionPlanID=@SubscriptionPlanID;";

        public override void Save(SubscriptionPlan entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SubscriptionPlanID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SubscriptionPlanID", MySqlDbType.Int32).Value = entity.SubscriptionPlanID;
            }

            cmd.Parameters.Add("@SubscriptionPlanName", MySqlDbType.VarChar).Value = entity.SubscriptionPlanName;
            cmd.Parameters.Add("@StartSubscriptionPlanItemID", MySqlDbType.Int32).Value = entity.StartSubscriptionPlanItemID;

            if (entity.SubscriptionPlanID == null)
            {
                entity.SubscriptionPlanID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SubscriptionPlan({0}) was not found in database.", entity.SubscriptionPlanID));
                }
            }
        }

        public override SubscriptionPlan Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SubscriptionPlanID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SubscriptionPlan Load(DataRow row)
        {
            SubscriptionPlan res = new SubscriptionPlan();

            if (!(row["SubscriptionPlanID"] is DBNull))
                res.SubscriptionPlanID = Convert.ToInt32(row["SubscriptionPlanID"]);
            if (!(row["SubscriptionPlanName"] is DBNull))
                res.SubscriptionPlanName = Convert.ToString(row["SubscriptionPlanName"]);
            if (!(row["StartSubscriptionPlanItemID"] is DBNull))
                res.StartSubscriptionPlanItemID = Convert.ToInt32(row["StartSubscriptionPlanItemID"]);

            return res;
        }
    }
}
