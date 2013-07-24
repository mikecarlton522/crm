using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class EmailQueueDataProvider : EntityDataProvider<EmailQueue>
    {
        private const string INSERT_COMMAND = "INSERT INTO EmailQueue(BillingID, SaleID, Completed, CreateDT) VALUES(@BillingID, @SaleID, @Completed, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE EmailQueue SET BillingID=@BillingID, SaleID=@SaleID, Completed=@Completed, CreateDT=@CreateDT WHERE EmailQueueID=@EmailQueueID;";
        private const string SELECT_COMMAND = "UPDATE EmailQueue SET BillingID=@BillingID, SaleID=@SaleID, Completed=@Completed, CreateDT=@CreateDT WHERE EmailQueueID=@EmailQueueID;";

        public override void Save(EmailQueue entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.EmailQueueID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@EmailQueueID", MySqlDbType.Int64).Value = entity.EmailQueueID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.DateTime).Value = entity.CreateDT;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;

            if (entity.EmailQueueID == null)
            {
                entity.EmailQueueID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("EmailQueue({0}) was not found in database.", entity.EmailQueueID));
                }
            }
        }

        public override EmailQueue Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@EmailQueueID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override EmailQueue Load(DataRow row)
        {
            EmailQueue res = new EmailQueue();

            if (!(row["EmailQueueID"] is DBNull))
                res.EmailQueueID = Convert.ToInt32(row["EmailQueueID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);

            return res;
        }
    }
}
