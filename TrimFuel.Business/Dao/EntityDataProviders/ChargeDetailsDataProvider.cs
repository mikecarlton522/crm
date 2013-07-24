using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargeDetailsDataProvider : EntityDataProvider<ChargeDetails>
    {
        private ChargeHistoryExDataProvider chargeHistoryExDataProvider = new ChargeHistoryExDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO ChargeDetails(ChargeHistoryID, SaleTypeID, SKU) VALUES(@ChargeHistoryID, @SaleTypeID, @SKU);";
        private const string UPDATE_COMMAND = "UPDATE ChargeDetails SET SaleTypeID=@SaleTypeID, SKU=@SKU WHERE ChargeHistoryID=@ChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ChargeHistoryEx ch INNER JOIN ChargeDetails cd ON cd.ChargeHistoryID = ch.ChargeHistoryID WHERE ch.ChargeHistoryID = @ChargeHistoryID;";

        public override void Save(ChargeDetails entity, IMySqlCommandCreater cmdCreater)
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
            cmd.Parameters.Add("@SaleTypeID", MySqlDbType.Int32).Value = entity.SaleTypeID;
            cmd.Parameters.Add("@SKU", MySqlDbType.VarChar).Value = entity.SKU;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("ChargeDetails({0}) was not found in database.", entity.ChargeHistoryID));
            }
        }

        public override ChargeDetails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargeDetails Load(DataRow row)
        {
            ChargeHistoryEx chargeHistory = (new ChargeHistoryExDataProvider()).Load(row);

            ChargeDetails res = new ChargeDetails();
            res.FillFromChargeHistory(chargeHistory);

            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt32(row["SaleTypeID"]);
            if (!(row["SKU"] is DBNull))
                res.SKU = Convert.ToString(row["SKU"]);

            return res;
        }
    }
}
