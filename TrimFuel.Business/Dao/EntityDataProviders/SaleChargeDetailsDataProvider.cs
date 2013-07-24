using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleChargeDetailsDataProvider : EntityDataProvider<SaleChargeDetails>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleChargeDetails(SaleID, SaleChargeTypeID, Amount, CurrencyID, CurrencyAmount, Description) VALUES(@SaleID, @SaleChargeTypeID, @Amount, @CurrencyID, @CurrencyAmount, @Description); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleChargeDetails SET SaleID=@SaleID, SaleChargeTypeID=@SaleChargeTypeID, Amount=@Amount, CurrencyID=@CurrencyID, CurrencyAmount=@CurrencyAmount, Description=@Description WHERE SaleChargeDetailsID=@SaleChargeDetailsID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleChargeDetails WHERE SaleChargeDetailsID = @SaleChargeDetailsID;";

        public override void Save(SaleChargeDetails entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleChargeDetailsID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleChargeDetailsID", MySqlDbType.Int64).Value = entity.SaleChargeDetailsID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@SaleChargeTypeID", MySqlDbType.Int32).Value = entity.SaleChargeTypeID;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = entity.CurrencyID;
            cmd.Parameters.Add("@CurrencyAmount", MySqlDbType.Decimal).Value = entity.CurrencyAmount;
            cmd.Parameters.Add("@Description", MySqlDbType.VarChar).Value = entity.Description;

            if (entity.SaleChargeDetailsID == null)
            {
                entity.SaleChargeDetailsID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleChargeDetails({0}) was not found in database.", entity.SaleChargeDetailsID));
                }
            }
        }

        public override SaleChargeDetails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleChargeDetailsID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleChargeDetails Load(DataRow row)
        {
            SaleChargeDetails res = new SaleChargeDetails();

            if (!(row["SaleChargeDetailsID"] is DBNull))
                res.SaleChargeDetailsID = Convert.ToInt64(row["SaleChargeDetailsID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["SaleChargeTypeID"] is DBNull))
                res.SaleChargeTypeID = Convert.ToInt32(row["SaleChargeTypeID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["CurrencyAmount"] is DBNull))
                res.CurrencyAmount = Convert.ToDecimal(row["CurrencyAmount"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);

            return res;
        }
    }
}
