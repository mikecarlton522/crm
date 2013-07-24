using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class EmergencyQueueDataProvider : EntityDataProvider<EmergencyQueue>
    {
        private const string SELECT_COMMAND = "SELECT * FROM EmergencyQueue WHERE EmergencyQueueID=@EmergencyQueueID;";
        private const string INSERT_COMMAND = "INSERT INTO EmergencyQueue(BillingID, Amount, CreateDT, Completed) VALUES(@BillingID, @Amount, @CreateDT, @Completed); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE EmergencyQueue SET BillingID=@BillingID, Amount=@Amount, CreateDT=@CreateDT, Completed=@Completed WHERE EmergencyQueueID=@EmergencyQueueID;";

        public override void Save(EmergencyQueue entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.EmergencyQueueID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@EmergencyQueueID", MySqlDbType.Int32).Value = entity.EmergencyQueueID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = entity.BillingID;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;

            if (entity.EmergencyQueueID == null)
            {
                entity.EmergencyQueueID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("EmergencyQueue({0}) was not found in database.", entity.EmergencyQueueID));
                }
            }
        }

        public override EmergencyQueue Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@EmergencyQueueID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override EmergencyQueue Load(DataRow row)
        {
            EmergencyQueue res = new EmergencyQueue();

            if (!(row["EmergencyQueueID"] is DBNull))
                res.EmergencyQueueID = Convert.ToInt32(row["EmergencyQueueID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
