using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class FaildChargeHistoryDataProvider : EntityDataProvider<FailedChargeHistory>
    {
        private const string INSERT_COMMAND = "INSERT INTO FailedChargeHistory(BillingID, ChargeDate, Amount, Response, Success, SaleTypeID, MerchantAccountID) VALUES(@BillingID, @ChargeDate, @Amount, @Response, @Success, @SaleTypeID, @MerchantAccountID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE FailedChargeHistory SET BillingID=@BillingID, ChargeDate=@ChargeDate, Amount=@Amount, Response=@Response, Success=@Success, SaleTypeID=@SaleTypeID, MerchantAccountID=@MerchantAccountID WHERE FailedChargeHistoryID=@FailedChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM FailedChargeHistory WHERE FailedChargeHistoryID=@FailedChargeHistoryID;";

        public override void Save(FailedChargeHistory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.FailedChargeHistoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int32).Value = entity.FailedChargeHistoryID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@ChargeDate", MySqlDbType.Timestamp).Value = entity.ChargeDate;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;
            cmd.Parameters.Add("@Success", MySqlDbType.Bit).Value = entity.Success;
            cmd.Parameters.Add("@SaleTypeID", MySqlDbType.Int32).Value = entity.SaleTypeID;
            cmd.Parameters.Add("@MerchantAccountID", MySqlDbType.Int32).Value = entity.MerchantAccountID;

            if (entity.FailedChargeHistoryID == null)
            {
                entity.FailedChargeHistoryID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("FailedChargeHistory({0}) was not found in database.", entity.FailedChargeHistoryID));
                }
            }
        }

        public override FailedChargeHistory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override FailedChargeHistory Load(DataRow row)
        {
            FailedChargeHistory res = new FailedChargeHistory();

            if (!(row["FailedChargeHistoryID"] is DBNull))
                res.FailedChargeHistoryID = Convert.ToInt32(row["FailedChargeHistoryID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["ChargeDate"] is DBNull))
                res.ChargeDate = Convert.ToDateTime(row["ChargeDate"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["Success"] is DBNull))
                res.Success = Convert.ToBoolean(row["Success"]);
            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt32(row["SaleTypeID"]);
            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);

            return res;
        }
    }
}
