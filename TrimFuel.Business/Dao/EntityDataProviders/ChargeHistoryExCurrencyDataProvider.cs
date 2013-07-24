using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargeHistoryExCurrencyDataProvider : EntityDataProvider<ChargeHistoryExCurrency>
    {
        private const string INSERT_COMMAND = "INSERT INTO ChargeHistoryExCurrency(ChargeHistoryID, CurrencyID, CurrencyAmount) VALUES(@ChargeHistoryID, @CurrencyID, @CurrencyAmount);";
        private const string UPDATE_COMMAND = "UPDATE ChargeHistoryExCurrency SET CurrencyID=@CurrencyID, CurrencyAmount=@CurrencyAmount WHERE ChargeHistoryID=@ChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ChargeHistoryExCurrency WHERE ChargeHistoryID = @ChargeHistoryID;";

        public override void Save(ChargeHistoryExCurrency entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            //ChargeHistoryEx 1 <-> 0..1 ChargeHistoryExCurrency association
            //Try to update first
            cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = entity.CurrencyID;
            cmd.Parameters.Add("@CurrencyAmount", MySqlDbType.Decimal).Value = entity.CurrencyAmount;

            //ChargeHistoryEx 1 <-> 0..1 ChargeHistoryExCurrency association
            if (cmd.ExecuteNonQuery() == 0)
            {
                //ChargeHistoryEx 1 <-> 0..1 ChargeHistoryExCurrency association
                //If update failed try to insert
                cmd.CommandText = INSERT_COMMAND;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Foreign Key ChargeHistoryEx({0}) was not found in database.", entity.ChargeHistoryID));
                }
            }
        }

        public override ChargeHistoryExCurrency Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargeHistoryExCurrency Load(DataRow row)
        {
            ChargeHistoryExCurrency res = new ChargeHistoryExCurrency();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["CurrencyAmount"] is DBNull))
                res.CurrencyAmount = Convert.ToDecimal(row["CurrencyAmount"]);

            return res;
        }
    }
}
