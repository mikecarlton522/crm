using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AuthOnlyChargeDetailsDataProvider : EntityDataProvider<AuthOnlyChargeDetails>
    {
        private ChargeHistoryExDataProvider chargeHistoryExDataProvider = new ChargeHistoryExDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO AuthOnlyChargeDetails(ChargeHistoryID, RequestedAmount, RequestedCurrencyID, RequestedCurrencyAmount) VALUES(@ChargeHistoryID, @RequestedAmount, @RequestedCurrencyID, @RequestedCurrencyAmount);";
        private const string UPDATE_COMMAND = "UPDATE AuthOnlyChargeDetails SET RequestedAmount=@RequestedAmount, RequestedCurrencyID=@RequestedCurrencyID, RequestedCurrencyAmount=@RequestedCurrencyAmount WHERE ChargeHistoryID=@ChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ChargeHistoryEx ch INNER JOIN AuthOnlyChargeDetails cd ON cd.ChargeHistoryID = ch.ChargeHistoryID WHERE ch.ChargeHistoryID = @ChargeHistoryID;";

        public override void Save(AuthOnlyChargeDetails entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ChargeHistoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            chargeHistoryExDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@RequestedAmount", MySqlDbType.Decimal).Value = entity.RequestedAmount;
            cmd.Parameters.Add("@RequestedCurrencyID", MySqlDbType.Int32).Value = entity.RequestedCurrencyID;
            cmd.Parameters.Add("@RequestedCurrencyAmount", MySqlDbType.Decimal).Value = entity.RequestedCurrencyAmount;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("AuthOnlyChargeDetails({0}) was not found in database.", entity.ChargeHistoryID));
            }
        }

        public override AuthOnlyChargeDetails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AuthOnlyChargeDetails Load(DataRow row)
        {
            ChargeHistoryEx chargeHistory = (new ChargeHistoryExDataProvider()).Load(row);

            AuthOnlyChargeDetails res = new AuthOnlyChargeDetails();
            res.FillFromChargeHistory(chargeHistory);

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
