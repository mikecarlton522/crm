using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ExtraTrialShipSaleDataProvider : EntityDataProvider<ExtraTrialShipSale>
    {
        private SaleDataProvider saleDataProvider = new SaleDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO ExtraTrialShipSale(SaleID, ExtraTrialShipID, BillingID) VALUES(@SaleID, @ExtraTrialShipID, @BillingID);";
        private const string UPDATE_COMMAND = "UPDATE UpsellSale SET ExtraTrialShipID=@ExtraTrialShipID, BillingID=@BillingID WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM Sale s INNER JOIN ExtraTrialShipSale es ON es.SaleID = s.SaleID WHERE s.SaleID = @SaleID;";

        public override void Save(ExtraTrialShipSale entity, IMySqlCommandCreater cmdCreater)
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
            cmd.Parameters.Add("@ExtraTrialShipID", MySqlDbType.Int32).Value = entity.ExtraTrialShipID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("ExtraTrialShipSale({0}) was not found in database.", entity.SaleID));
            }
        }

        public override ExtraTrialShipSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ExtraTrialShipSale Load(DataRow row)
        {
            Sale sale = (new SaleDataProvider()).Load(row);

            ExtraTrialShipSale res = new ExtraTrialShipSale();
            res.FillFromSale(sale);

            if (!(row["ExtraTrialShipID"] is DBNull))
                res.ExtraTrialShipID = Convert.ToInt32(row["ExtraTrialShipID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);

            return res;
        }
    }
}
