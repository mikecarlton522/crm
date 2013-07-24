using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingStopChargeDataProvider : EntityDataProvider<BillingStopCharge>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingStopCharge(BillingID, StopReason, CreateDT) VALUES(@BillingID, @StopReason, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingStopCharge SET BillingID=@BillingID, StopReason=@StopReason, CreateDT=@CreateDT WHERE BillingStopChargeID=@BillingStopChargeID;";
        private const string SELECT_COMMAND = "SELECT * FROM BillingStopCharge WHERE BillingStopChargeID=@BillingStopChargeID;";

        public override void Save(BillingStopCharge entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingStopChargeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingStopChargeID", MySqlDbType.Int64).Value = entity.BillingStopChargeID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@StopReason", MySqlDbType.VarChar).Value = entity.StopReason;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.BillingStopChargeID == null)
            {
                entity.BillingStopChargeID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingStopCharge({0}) was not found in database.", entity.BillingStopChargeID));
                }
            }
        }

        public override BillingStopCharge Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingStopChargeID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingStopCharge Load(DataRow row)
        {
            BillingStopCharge res = new BillingStopCharge();

            if (!(row["BillingStopChargeID"] is DBNull))
                res.BillingStopChargeID = Convert.ToInt64(row["BillingStopChargeID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["StopReason"] is DBNull))
                res.StopReason = Convert.ToString(row["StopReason"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}