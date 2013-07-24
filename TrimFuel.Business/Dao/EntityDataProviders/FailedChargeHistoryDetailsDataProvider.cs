using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class FailedChargeHistoryDetailsDataProvider : EntityDataProvider<FailedChargeHistoryDetails>
    {
        private FaildChargeHistoryDataProvider failedChargeHistoryExDataProvider = new FaildChargeHistoryDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO FailedChargeHistoryDetails(FailedChargeHistoryID, SubscriptionID, SaleTypeID, SKU) VALUES(@FailedChargeHistoryID, @SubscriptionID, @SaleTypeID, @SKU);";
        private const string UPDATE_COMMAND = "UPDATE FailedChargeHistoryDetails SET SaleTypeID=@SaleTypeID, SKU=@SKU, SubscriptionID=@SubscriptionID WHERE FailedChargeHistoryID=@FailedChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM FailedChargeHistoryDetails ch INNER JOIN FailedChargeHistory cd ON cd.FailedChargeHistoryID = ch.FailedChargeHistoryID WHERE ch.FailedChargeHistoryID = @FailedChargeHistoryID;";

        public override void Save(FailedChargeHistoryDetails entity, IMySqlCommandCreater cmdCreater)
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

            failedChargeHistoryExDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int64).Value = entity.FailedChargeHistoryID;
            cmd.Parameters.Add("@SaleTypeID", MySqlDbType.Int32).Value = entity.SaleTypeID;
            cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = entity.SubscriptionID;
            cmd.Parameters.Add("@SKU", MySqlDbType.VarChar).Value = entity.SKU;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("Failed Charge History Details ({0}) was not found in database.", entity.FailedChargeHistoryID));
            }
        }

        public override FailedChargeHistoryDetails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override FailedChargeHistoryDetails Load(System.Data.DataRow row)
        {
            FailedChargeHistory failedChargeHistory = (new FaildChargeHistoryDataProvider()).Load(row);

            FailedChargeHistoryDetails res = new FailedChargeHistoryDetails();
            res.FillFromChargeHistory(failedChargeHistory);

            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt32(row["SaleTypeID"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["SKU"] is DBNull))
                res.SKU = Convert.ToString(row["SKU"]);

            return res;
        }
    }
}
