using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PromoGiftSaleDataProvider : EntityDataProvider<PromoGiftSale>
    {
        private const string INSERT_COMMAND = "INSERT INTO PromoGiftSale(PromoGiftID, SaleID, RedeemAmount) VALUES(@PromoGiftID, @SaleID, @RedeemAmount);";

        public override void Save(PromoGiftSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            cmd.CommandText = INSERT_COMMAND;

            cmd.Parameters.Add("@PromoGiftID", MySqlDbType.Int64).Value = entity.PromoGiftID;
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RedeemAmount", MySqlDbType.Decimal).Value = entity.RedeemAmount;

            cmd.ExecuteNonQuery();
        }

        public override PromoGiftSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override PromoGiftSale Load(DataRow row)
        {
            PromoGiftSale res = new PromoGiftSale();

            if (!(row["PromoGiftID"] is DBNull))
                res.PromoGiftID = Convert.ToInt64(row["PromoGiftID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["RedeemAmount"] is DBNull))
                res.RedeemAmount = Convert.ToDecimal(row["RedeemAmount"]);

            return res;
        }
    }
}
