using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class UpsellSaleDataProvider : EntityDataProvider<UpsellSale>
    {
        private SaleDataProvider saleDataProvider = new SaleDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO UpsellSale(SaleID, UpsellID, ChargeHistoryID, PaygeaID) VALUES(@SaleID, @UpsellID, @ChargeHistoryID, @PaygeaID);";
        private const string UPDATE_COMMAND = "UPDATE UpsellSale SET UpsellID=@UpsellID, ChargeHistoryID=@ChargeHistoryID, PaygeaID=@PaygeaID WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM Sale s INNER JOIN UpsellSale us ON us.SaleID = s.SaleID WHERE s.SaleID = @SaleID;";

        public override void Save(UpsellSale entity, IMySqlCommandCreater cmdCreater)
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
            cmd.Parameters.Add("@UpsellID", MySqlDbType.Int32).Value = entity.UpsellID;
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@PaygeaID", MySqlDbType.Int64).Value = entity.PaygeaID;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("BillingSale({0}) was not found in database.", entity.SaleID));
            }
        }

        public override UpsellSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override UpsellSale Load(DataRow row)
        {
            Sale sale = (new SaleDataProvider()).Load(row);

            UpsellSale res = new UpsellSale();
            res.FillFromSale(sale);

            if (!(row["UpsellID"] is DBNull))
                res.UpsellID = Convert.ToInt32(row["UpsellID"]);
            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["PaygeaID"] is DBNull))
                res.PaygeaID = Convert.ToInt64(row["PaygeaID"]);

            return res;
        }
    }
}
