using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductSKUDataProvider : EntityDataProvider<ProductSKU>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductSKU(ProductSKU, ProductName) VALUES(@ProductSKU, @ProductName);";
        private const string UPDATE_COMMAND = "UPDATE ProductSKU SET ProductName=@ProductName WHERE ProductSKU=@ProductSKU;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductSKU WHERE ProductSKU=@ProductSKU;";

        public override void Save(ProductSKU entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var existingEntity = Load(entity.ProductSKU_, cmdCreater);

            if (existingEntity == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = entity.ProductSKU_;
            cmd.Parameters.Add("@ProductName", MySqlDbType.VarChar).Value = entity.ProductName;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("ProductSKU({0}) was not found in database.", entity.ProductSKU_));
            }
        }

        public override ProductSKU Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductSKU Load(System.Data.DataRow row)
        {
            ProductSKU res = new ProductSKU();

            if (!(row["ProductSKU"] is DBNull))
                res.ProductSKU_ = Convert.ToString(row["ProductSKU"]);
            if (!(row["ProductName"] is DBNull))
                res.ProductName = Convert.ToString(row["ProductName"]);

            return res;
        }
    }
}
