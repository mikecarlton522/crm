using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductCodeDataProvider : EntityDataProvider<ProductCode>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductCode(ProductCode, Name) VALUES(@ProductCode, @Name); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ProductCode SET ProductCode=@ProductCode, Name=@Name WHERE ProductCodeID=@ProductCodeID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductCode WHERE ProductCodeID=@ProductCodeID;";

        public override void Save(ProductCode entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductCodeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int64).Value = entity.ProductCodeID;
            }

            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode_;
            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;

            if (entity.ProductCodeID == null)
            {
                entity.ProductCodeID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductCode ({0}) was not found in database.", entity.ProductCodeID));
                }
            }
        }

        public override ProductCode Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductCode Load(DataRow row)
        {
            ProductCode res = new ProductCode();

            if (!(row["ProductCodeID"] is DBNull))
                res.ProductCodeID = Convert.ToInt32(row["ProductCodeID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode_ = Convert.ToString(row["ProductCode"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);

            return res;
        }
    }
}
