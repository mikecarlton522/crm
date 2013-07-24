using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AuthOnlyFailedChargeDetailsDataProvider : EntityDataProvider<AuthOnlyFailedChargeDetails>
    {
        private FaildChargeHistoryDataProvider failedChargeHistoryDataProvider = new FaildChargeHistoryDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO AuthOnlyFailedChargeDetails(FailedChargeHistoryID, RequestedAmount, RequestedCurrencyID, RequestedCurrencyAmount) VALUES(@FailedChargeHistoryID, @RequestedAmount, @RequestedCurrencyID, @RequestedCurrencyAmount);";
        private const string UPDATE_COMMAND = "UPDATE AuthOnlyFailedChargeDetails SET RequestedAmount=@RequestedAmount, RequestedCurrencyID=@RequestedCurrencyID, RequestedCurrencyAmount=@RequestedCurrencyAmount WHERE FailedChargeHistoryID=@FailedChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM FailedChargeHistory ch INNER JOIN AuthOnlyFailedChargeDetails cd ON cd.FailedChargeHistoryID = ch.FailedChargeHistoryID WHERE ch.FailedChargeHistoryID = @FailedChargeHistoryID;";

        public override void Save(AuthOnlyFailedChargeDetails entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.FailedChargeHistoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            failedChargeHistoryDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int32).Value = entity.FailedChargeHistoryID;
            cmd.Parameters.Add("@RequestedAmount", MySqlDbType.Decimal).Value = entity.RequestedAmount;
            cmd.Parameters.Add("@RequestedCurrencyID", MySqlDbType.Int32).Value = entity.RequestedCurrencyID;
            cmd.Parameters.Add("@RequestedCurrencyAmount", MySqlDbType.Decimal).Value = entity.RequestedCurrencyAmount;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("AuthOnlyFailedChargeDetails({0}) was not found in database.", entity.FailedChargeHistoryID));
            }
        }

        public override AuthOnlyFailedChargeDetails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AuthOnlyFailedChargeDetails Load(DataRow row)
        {
            FailedChargeHistory failedChargeHistory = (new FaildChargeHistoryDataProvider()).Load(row);

            AuthOnlyFailedChargeDetails res = new AuthOnlyFailedChargeDetails();
            res.FillFromChargeHistory(failedChargeHistory);

            if (!(row["RequestedAmount"] is DBNull))
                res.RequestedAmount = Convert.ToDecimal(row["RequestedAmount"]);
            if (!(row["RequestedCurrencyID"] is DBNull))
                res.RequestedCurrencyID = Convert.ToInt32(row["RequestedCurrencyID"]);
            if (!(row["RequestedCurrencyAmount"] is DBNull))
                res.RequestedCurrencyAmount = Convert.ToDecimal(row["RequestedCurrencyAmount"]);

            return res;
        }
    }
}
