using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RecurringPlanDataProvider : EntityDataProvider<RecurringPlan>
    {
        private const string INSERT_COMMAND = "INSERT INTO RecurringPlan(ProductID, Name) VALUES(@ProductID, @Name); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RecurringPlan SET ProductID=@ProductID, Name=@Name WHERE RecurringPlanID=@RecurringPlanID;";
        private const string SELECT_COMMAND = "SELECT * FROM RecurringPlan WHERE RecurringPlanID=@RecurringPlanID;";

        public override void Save(RecurringPlan entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RecurringPlanID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RecurringPlanID", MySqlDbType.Int32).Value = entity.RecurringPlanID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;

            if (entity.RecurringPlanID == null)
            {
                entity.RecurringPlanID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RecurringPlan({0}) was not found in database.", entity.RecurringPlanID));
                }
            }
        }

        public override RecurringPlan Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RecurringPlanID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RecurringPlan Load(DataRow row)
        {
            RecurringPlan res = new RecurringPlan();

            if (!(row["RecurringPlanID"] is DBNull))
                res.RecurringPlanID = Convert.ToInt32(row["RecurringPlanID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);

            return res;
        }
    }
}
