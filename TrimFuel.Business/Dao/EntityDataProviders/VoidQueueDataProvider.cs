using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class VoidQueueDataProvider : EntityDataProvider<VoidQueue>
    {
        private const string INSERT_COMMAND = "INSERT INTO VoidQueue(BillingID, SaleID, Amount, Completed, SaleChargeDT, CreateDT) VALUES(@BillingID, @SaleID, @Amount, @Completed, @SaleChargeDT, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE VoidQueue SET BillingID=@BillingID, SaleID=@SaleID, Amount=@Amount, Completed=@Completed, SaleChargeDT=@SaleChargeDT, CreateDT=@CreateDT WHERE VoidQueueID=@VoidQueueID;";
        private const string SELECT_COMMAND = "SELECT * FROM VoidQueue WHERE VoidQueueID=@VoidQueueID;";

        public override void Save(VoidQueue entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.VoidQueueID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@VoidQueueID", MySqlDbType.Int32).Value = entity.VoidQueueID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;
            cmd.Parameters.Add("@SaleChargeDT", MySqlDbType.Timestamp).Value = entity.SaleChargeDT;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.VoidQueueID == null)
            {
                entity.VoidQueueID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("VoidQueue({0}) was not found in database.", entity.VoidQueueID));
                }
            }
        }

        public override VoidQueue Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@VoidQueueID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override VoidQueue Load(DataRow row)
        {
            VoidQueue res = new VoidQueue();

            if (!(row["VoidQueueID"] is DBNull))
                res.VoidQueueID = Convert.ToInt32(row["VoidQueueID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["SaleChargeDT"] is DBNull))
                res.SaleChargeDT = Convert.ToDateTime(row["SaleChargeDT"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
