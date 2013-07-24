using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;


namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductRoutingDataProvider : EntityDataProvider<ProductRouting>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductRouting(ProductID, RoutingURL) VALUES(@ProductID, @RoutingURL); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ProductRouting SET RoutingURL=@RoutingURL, ProductID=@ProductID WHERE ProductID=@IDProductID";
        private const string SELECT_COMMAND = "SELECT * FROM ProductRouting WHERE ProductID=@ProductID";

        public override void Save(ProductRouting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductRoutingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDProductID", MySqlDbType.Int32).Value = entity.ProductRoutingID.Value.ProductID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@RoutingURL", MySqlDbType.VarChar).Value = entity.RoutingURL;

            if (entity.ProductRoutingID == null)
            {
                cmd.ExecuteNonQuery();
                entity.ProductRoutingID = new ProductRouting.ID(){ ProductID = entity.ProductID.Value};
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format(" ProductRouting({0}) was not found in database.", entity.ProductRoutingID.Value.ProductID));
                }
                else
                {
                    entity.ProductRoutingID = new ProductRouting.ID() { ProductID = entity.ProductID.Value };
                }
            }
        }

        public override ProductRouting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = ((ProductRouting.ID?)key).Value.ProductID;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductRouting Load(DataRow row)
        {
            ProductRouting res = new ProductRouting();

            if (!(row["ProductID"] is DBNull))
                res.ProductRoutingID = new ProductRouting.ID
                {
                    ProductID = Convert.ToInt32(row["ProductID"])
                };
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["RoutingURL"] is DBNull))
                res.RoutingURL = Convert.ToString(row["RoutingURL"]);

            return res;
        }
    }
}
