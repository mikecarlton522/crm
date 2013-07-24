using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RefererCommissionDataProvider : EntityDataProvider<RefererCommission>
    {
        private const string INSERT_COMMAND = "INSERT INTO RefererCommission(RefererID, RefererCommissionTID, Amount, Completed, CreateDT, RemainingAmount) VALUES(@RefererID, @RefererCommissionTID, @Amount, @Completed, @CreateDT, @RemainingAmount); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RefererCommission SET RefererID=@RefererID, RefererCommissionTID=@RefererCommissionTID, Amount=@Amount, Completed=@Completed, CreateDT=@CreateDT, RemainingAmount=@RemainingAmount WHERE RefererCommissionID=@RefererCommissionID;";
        private const string SELECT_COMMAND = "SELECT * FROM RefererCommission WHERE RefererCommissionID=@RefererCommissionID;";

        public override void Save(RefererCommission entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RefererCommissionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RefererCommissionID", MySqlDbType.Int32).Value = entity.RefererCommissionID;
            }

            cmd.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = entity.RefererID;
            cmd.Parameters.Add("@RefererCommissionTID", MySqlDbType.Int32).Value = entity.RefererCommissionTID;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@RemainingAmount", MySqlDbType.Decimal).Value = entity.RemainingAmount;

            if (entity.RefererCommissionID == null)
            {
                entity.RefererCommissionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RefererCommission({0}) was not found in database.", entity.RefererCommissionID));
                }
            }
        }

        public override RefererCommission Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RefererCommissionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RefererCommission Load(DataRow row)
        {
            RefererCommission res = new RefererCommission();

            if (!(row["RefererCommissionID"] is DBNull))
                res.RefererCommissionID = Convert.ToInt32(row["RefererCommissionID"]);
            if (!(row["RefererID"] is DBNull))
                res.RefererID = Convert.ToInt32(row["RefererID"]);
            if (!(row["RefererCommissionTID"] is DBNull))
                res.RefererCommissionTID = Convert.ToInt32(row["RefererCommissionTID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["RemainingAmount"] is DBNull))
                res.RemainingAmount = Convert.ToDecimal(row["RemainingAmount"]);

            return res;
        }
    }
}
