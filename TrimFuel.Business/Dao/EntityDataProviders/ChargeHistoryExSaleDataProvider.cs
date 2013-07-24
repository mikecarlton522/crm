using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargeHistoryExSaleDataProvider : EntityDataProvider<ChargeHistoryExSale>
    {
        private const string INSERT_COMMAND = "INSERT INTO ChargeHistoryExSale(ChargeHistoryID, SaleID, Amount, CurrencyID, CurrencyAmount) VALUES(@ChargeHistoryID, @SaleID, @Amount, @CurrencyID, @CurrencyAmount);";

        public override void Save(ChargeHistoryExSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ChargeHistoryExSaleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                throw new NotImplementedException();
            }

            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = entity.CurrencyID;
            cmd.Parameters.Add("@CurrencyAmount", MySqlDbType.Decimal).Value = entity.CurrencyAmount;

            if (entity.ChargeHistoryExSaleID == null)
            {
                cmd.ExecuteNonQuery();
                entity.ChargeHistoryExSaleID = new ChargeHistoryExSale.ID() { ChargeHistoryID = entity.ChargeHistoryID.Value, SaleID = entity.SaleID.Value };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override ChargeHistoryExSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override ChargeHistoryExSale Load(DataRow row)
        {
            ChargeHistoryExSale res = new ChargeHistoryExSale();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["CurrencyAmount"] is DBNull))
                res.CurrencyAmount = Convert.ToDecimal(row["CurrencyAmount"]);
            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt32(row["SaleID"]);

            res.ChargeHistoryExSaleID = new ChargeHistoryExSale.ID()
            {
                ChargeHistoryID = res.ChargeHistoryID == null ? 0 : res.ChargeHistoryID.Value,
                SaleID = res.SaleID == null ? 0 : res.SaleID.Value
            };

            return res;
        }
    }
}
