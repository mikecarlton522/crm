using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingSaleDataProvider : EntityDataProvider<BillingSale>
    {
        private SaleDataProvider saleDataProvider = new SaleDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO BillingSale(SaleID, BillingSubscriptionID, ChargeHistoryID, PaygeaID, RebillCycle, ProductCode, Quantity) VALUES(@SaleID, @BillingSubscriptionID, @ChargeHistoryID, @PaygeaID, @RebillCycle, @ProductCode, @Quantity);";
        private const string UPDATE_COMMAND = "UPDATE BillingSale SET BillingSubscriptionID=@BillingSubscriptionID, ChargeHistoryID=@ChargeHistoryID, PaygeaID=@PaygeaID, RebillCycle=@RebillCycle, ProductCode=@ProductCode, Quantity=@Quantity WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM Sale s INNER JOIN BillingSale bs ON bs.SaleID = s.SaleID WHERE s.SaleID = @SaleID;";

        public override void Save(BillingSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            saleDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@PaygeaID", MySqlDbType.Int64).Value = entity.PaygeaID;
            cmd.Parameters.Add("@RebillCycle", MySqlDbType.Int32).Value = entity.RebillCycle;
            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("BillingSale({0}) was not found in database.", entity.SaleID));
            }
        }

        public override BillingSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingSale Load(DataRow row)
        {
            Sale sale = (new SaleDataProvider()).Load(row);

            BillingSale res = new BillingSale();
            res.FillFromSale(sale);

            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["PaygeaID"] is DBNull))
                res.PaygeaID = Convert.ToInt64(row["PaygeaID"]);
            if (!(row["RebillCycle"] is DBNull))
                res.RebillCycle = Convert.ToInt32(row["RebillCycle"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
