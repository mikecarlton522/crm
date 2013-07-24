using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class MerchantAccountProductDataProvider : EntityDataProvider<MerchantAccountProduct>
    {
        private const string SELECT_COMMAND = "SELECT * FROM MerchantAccountProduct WHERE MerchantAccountProductID=@MerchantAccountProductID;";

        public override void Save(MerchantAccountProduct entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override MerchantAccountProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@MerchantAccountProductID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override MerchantAccountProduct Load(DataRow row)
        {
            MerchantAccountProduct res = new MerchantAccountProduct();

            if (!(row["MerchantAccountProductID"] is DBNull))
                res.MerchantAccountProductID = Convert.ToInt32(row["MerchantAccountProductID"]);
            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt16(row["Percentage"]);
            if (!(row["UseForRebill"] is DBNull))
                res.UseForRebill = Convert.ToBoolean(row["UseForRebill"]);
            if (!(row["OnlyRefundCredit"] is DBNull))
                res.OnlyRefundCredit = Convert.ToBoolean(row["OnlyRefundCredit"]);

            return res;
        }
    }
}
