using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargeHistoryExDataProvider : EntityDataProvider<ChargeHistoryEx>
    {
        private const string INSERT_COMMAND = "INSERT INTO ChargeHistoryEx(ChargeTypeID, MerchantAccountID, BillingSubscriptionID, ChargeDate, Amount, AuthorizationCode, TransactionNumber, Response, Success, ChildMID) VALUES(@ChargeTypeID, @MerchantAccountID, @BillingSubscriptionID, @ChargeDate, @Amount, @AuthorizationCode, @TransactionNumber, @Response, @Success, @ChildMID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ChargeHistoryEx SET ChargeTypeID=@ChargeTypeID, MerchantAccountID=@MerchantAccountID, BillingSubscriptionID=@BillingSubscriptionID, ChargeDate=@ChargeDate, Amount=@Amount, AuthorizationCode=@AuthorizationCode, TransactionNumber=@TransactionNumber, Response=@Response, Success=@Success, ChildMID=@ChildMID WHERE ChargeHistoryID=@ChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ChargeHistoryEx WHERE ChargeHistoryID = @ChargeHistoryID;";

        public override void Save(ChargeHistoryEx entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ChargeHistoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            }

            cmd.Parameters.Add("@ChargeTypeID", MySqlDbType.Int32).Value = entity.ChargeTypeID;
            cmd.Parameters.Add("@MerchantAccountID", MySqlDbType.Int32).Value = entity.MerchantAccountID;
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@ChargeDate", MySqlDbType.Timestamp).Value = entity.ChargeDate;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@AuthorizationCode", MySqlDbType.VarChar).Value = entity.AuthorizationCode;
            cmd.Parameters.Add("@TransactionNumber", MySqlDbType.VarChar).Value = entity.TransactionNumber;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;
            cmd.Parameters.Add("@Success", MySqlDbType.Bit).Value = entity.Success;
            cmd.Parameters.Add("@ChildMID", MySqlDbType.VarChar).Value = entity.ChildMID;

            if (entity.ChargeHistoryID == null)
            {
                entity.ChargeHistoryID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ChargeHistory({0}) was not found in database.", entity.ChargeHistoryID));
                }
            }
        }

        public override ChargeHistoryEx Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargeHistoryEx Load(DataRow row)
        {
            ChargeHistoryEx res = new ChargeHistoryEx();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["ChargeTypeID"] is DBNull))
                res.ChargeTypeID = Convert.ToInt32(row["ChargeTypeID"]);
            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);
            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["ChargeDate"] is DBNull))
                res.ChargeDate = Convert.ToDateTime(row["ChargeDate"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["AuthorizationCode"] is DBNull))
                res.AuthorizationCode = Convert.ToString(row["AuthorizationCode"]);
            if (!(row["TransactionNumber"] is DBNull))
                res.TransactionNumber = Convert.ToString(row["TransactionNumber"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["Success"] is DBNull))
                res.Success = Convert.ToBoolean(row["Success"]);
            if (!(row["ChildMID"] is DBNull))
                res.ChildMID = Convert.ToString(row["ChildMID"]);

            return res;
        }
    }
}
