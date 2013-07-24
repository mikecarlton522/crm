using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RecurringPlanCycleDataProvider : EntityDataProvider<RecurringPlanCycle>
    {
        private const string INSERT_COMMAND = "INSERT INTO RecurringPlanCycle(RecurringPlanID, Interim, RetryInterim, Cycle, Recurring) VALUES(@RecurringPlanID, @Interim, @RetryInterim, @Cycle, @Recurring); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RecurringPlanCycle SET RecurringPlanID=@RecurringPlanID, Interim=@Interim, RetryInterim=@RetryInterim, Cycle=@Cycle, Recurring=@Recurring WHERE RecurringPlanCycleID=@RecurringPlanCycleID;";
        private const string SELECT_COMMAND = "SELECT * FROM RecurringPlanCycle WHERE RecurringPlanCycleID=@RecurringPlanCycleID;";

        public override void Save(RecurringPlanCycle entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RecurringPlanCycleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RecurringPlanCycleID", MySqlDbType.Int32).Value = entity.RecurringPlanCycleID;
            }

            cmd.Parameters.Add("@RecurringPlanID", MySqlDbType.Int32).Value = entity.RecurringPlanID;
            cmd.Parameters.Add("@Interim", MySqlDbType.Int32).Value = entity.Interim;
            cmd.Parameters.Add("@RetryInterim", MySqlDbType.Int32).Value = entity.RetryInterim;
            cmd.Parameters.Add("@Cycle", MySqlDbType.Int32).Value = entity.Cycle;
            cmd.Parameters.Add("@Recurring", MySqlDbType.Bit).Value = entity.Recurring;

            if (entity.RecurringPlanCycleID == null)
            {
                entity.RecurringPlanCycleID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RecurringPlanCycle({0}) was not found in database.", entity.RecurringPlanCycleID));
                }
            }
        }

        public override RecurringPlanCycle Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RecurringPlanCycleID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RecurringPlanCycle Load(DataRow row)
        {
            RecurringPlanCycle res = new RecurringPlanCycle();

            if (!(row["RecurringPlanCycleID"] is DBNull))
                res.RecurringPlanCycleID = Convert.ToInt32(row["RecurringPlanCycleID"]);
            if (!(row["RecurringPlanID"] is DBNull))
                res.RecurringPlanID = Convert.ToInt32(row["RecurringPlanID"]);
            if (!(row["Interim"] is DBNull))
                res.Interim = Convert.ToInt32(row["Interim"]);
            if (!(row["RetryInterim"] is DBNull))
                res.RetryInterim = Convert.ToInt32(row["RetryInterim"]);
            if (!(row["Cycle"] is DBNull))
                res.Cycle = Convert.ToInt32(row["Cycle"]);
            if (!(row["Recurring"] is DBNull))
                res.Recurring = Convert.ToBoolean(row["Recurring"]);

            return res;
        }
    }
}
