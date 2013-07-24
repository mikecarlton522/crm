using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class WebStoreProductDataProvider : EntityDataProvider<WebStoreProduct>
    {
        private const string SELECT_COMMAND = "SELECT * FROM WebStoreProduct WHERE WebStoreProductID=@WebStoreProductID;";

        public override void Save(WebStoreProduct entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override WebStoreProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@WebStoreProductID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override WebStoreProduct Load(System.Data.DataRow row)
        {
            WebStoreProduct res = new WebStoreProduct();

            if (!(row["WebStoreProductID"] is DBNull))
                res.WebStoreProductID = Convert.ToInt32(row["WebStoreProductID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);

            return res;
        }
    }
}
