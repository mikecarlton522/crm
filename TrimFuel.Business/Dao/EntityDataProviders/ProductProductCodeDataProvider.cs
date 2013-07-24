using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductProductCodeDataProvider : EntityDataProvider<ProductProductCode>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductProductCode(ProductID, ProductCodeID) VALUES(@ProductID, @ProductCodeID);";
        private const string UPDATE_COMMAND = "UPDATE ProductProductCode SET ProductID=@ProductID, ProductCodeID=@ProductCodeID WHERE ProductID=@ProductID and ProductCodeID=@ProductCodeID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductProductCode WHERE ProductID=@ProductID and ProductCodeID=@ProductCodeID;";

        public override void Save(ProductProductCode entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductProductCodeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductProductCodeID.Value.ProductID;
                cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = entity.ProductProductCodeID.Value.ProductCodeID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = entity.ProductCodeID;

            cmd.ExecuteNonQuery();
            entity.ProductProductCodeID = new ProductProductCode.ID() { ProductCodeID = entity.ProductCodeID, ProductID = entity.ProductID };
        }

        public override ProductProductCode Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = ((ProductProductCode.ID?)key).Value.ProductCodeID;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = ((ProductProductCode.ID?)key).Value.ProductID;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductProductCode Load(DataRow row)
        {
            ProductProductCode res = new ProductProductCode();

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["ProductCodeID"] is DBNull))
                res.ProductCodeID = Convert.ToInt32(row["ProductCodeID"]);

            res.ProductProductCodeID = new ProductProductCode.ID() { ProductCodeID = res.ProductCodeID, ProductID = res.ProductID };

            return res;
        }
    }
}
