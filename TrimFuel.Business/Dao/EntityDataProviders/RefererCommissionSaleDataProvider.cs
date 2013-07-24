using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RefererCommissionSaleDataProvider : EntityDataProvider<RefererCommissionSale>
    {
        private const string INSERT_COMMAND = "INSERT INTO RefererCommissionSale(RefererCommissionID, SaleID, RedeemAmount) VALUES(@RefererCommissionID, @SaleID, @RedeemAmount);";

        public override void Save(RefererCommissionSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            cmd.CommandText = INSERT_COMMAND;

            cmd.Parameters.Add("@RefererCommissionID", MySqlDbType.Int32).Value = entity.RefererCommissionID;
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RedeemAmount", MySqlDbType.Decimal).Value = entity.RedeemAmount;

            cmd.ExecuteNonQuery();
        }

        public override RefererCommissionSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override RefererCommissionSale Load(DataRow row)
        {
            RefererCommissionSale res = new RefererCommissionSale();

            if (!(row["RefererCommissionID"] is DBNull))
                res.RefererCommissionID = Convert.ToInt32(row["RefererCommissionID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["RedeemAmount"] is DBNull))
                res.RedeemAmount = Convert.ToDecimal(row["RedeemAmount"]);

            return res;
        }
    }
}
