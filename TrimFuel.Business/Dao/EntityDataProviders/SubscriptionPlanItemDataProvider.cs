using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SubscriptionPlanItemDataProvider : EntityDataProvider<SubscriptionPlanItem>
    {
        private const string INSERT_COMMAND = "INSERT INTO SubscriptionPlanItem(NextSubscriptionPlanItemID, Interim) VALUES(@NextSubscriptionPlanItemID, @Interim); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SubscriptionPlanItem SET NextSubscriptionPlanItemID=@NextSubscriptionPlanItemID, Interim=@Interim WHERE SubscriptionPlanItemID=@SubscriptionPlanItemID;";
        private const string SELECT_COMMAND = "SELECT * FROM SubscriptionPlanItem WHERE SubscriptionPlanItemID=@SubscriptionPlanItemID;";

        public override void Save(SubscriptionPlanItem entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SubscriptionPlanItemID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SubscriptionPlanItemID", MySqlDbType.Int32).Value = entity.SubscriptionPlanItemID;
            }

            cmd.Parameters.Add("@NextSubscriptionPlanItemID", MySqlDbType.Int32).Value = entity.NextSubscriptionPlanItemID;
            cmd.Parameters.Add("@Interim", MySqlDbType.Int32).Value = entity.Interim;

            if (entity.SubscriptionPlanItemID == null)
            {
                entity.SubscriptionPlanItemID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SubscriptionPlanItem({0}) was not found in database.", entity.SubscriptionPlanItemID));
                }
            }
        }

        public override SubscriptionPlanItem Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SubscriptionPlanItemID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SubscriptionPlanItem Load(DataRow row)
        {
            SubscriptionPlanItem res = new SubscriptionPlanItem();

            if (!(row["SubscriptionPlanItemID"] is DBNull))
                res.SubscriptionPlanItemID = Convert.ToInt32(row["SubscriptionPlanItemID"]);
            if (!(row["NextSubscriptionPlanItemID"] is DBNull))
                res.NextSubscriptionPlanItemID = Convert.ToInt32(row["NextSubscriptionPlanItemID"]);
            if (!(row["Interim"] is DBNull))
                res.Interim = Convert.ToInt32(row["Interim"]);

            return res;
        }
    }
}
