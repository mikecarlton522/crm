using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class FailedChargeHistoryCurrencyDataProvider : EntityDataProvider<FailedChargeHistoryCurrency>
    {
        private const string INSERT_COMMAND = "INSERT INTO FailedChargeHistoryCurrency(FailedChargeHistoryID, CurrencyID, CurrencyAmount) VALUES(@FailedChargeHistoryID, @CurrencyID, @CurrencyAmount);";
        private const string UPDATE_COMMAND = "UPDATE FailedChargeHistoryCurrency SET CurrencyID=@CurrencyID, CurrencyAmount=@CurrencyAmount WHERE FailedChargeHistoryID=@FailedChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM FailedChargeHistoryCurrency WHERE FailedChargeHistoryID = @FailedChargeHistoryID;";

        public override void Save(FailedChargeHistoryCurrency entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            //FailedChargeHistory 1 <-> 0..1 FailedChargeHistoryCurrency association
            //Try to update first
            cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int32).Value = entity.FailedChargeHistoryID;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = entity.CurrencyID;
            cmd.Parameters.Add("@CurrencyAmount", MySqlDbType.Decimal).Value = entity.CurrencyAmount;

            //FailedChargeHistory 1 <-> 0..1 FailedChargeHistoryCurrency association
            if (cmd.ExecuteNonQuery() == 0)
            {
                //FailedChargeHistory 1 <-> 0..1 FailedChargeHistoryCurrency association
                //If update failed try to insert
                cmd.CommandText = INSERT_COMMAND;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Foreign Key FailedChargeHistory({0}) was not found in database.", entity.FailedChargeHistoryID));
                }
            }
        }

        public override FailedChargeHistoryCurrency Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@FailedChargeHistoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override FailedChargeHistoryCurrency Load(DataRow row)
        {
            FailedChargeHistoryCurrency res = new FailedChargeHistoryCurrency();

            if (!(row["FailedChargeHistoryID"] is DBNull))
                res.FailedChargeHistoryID = Convert.ToInt32(row["FailedChargeHistoryID"]);
            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["CurrencyAmount"] is DBNull))
                res.CurrencyAmount = Convert.ToDecimal(row["CurrencyAmount"]);

            return res;
        }
    }
}
