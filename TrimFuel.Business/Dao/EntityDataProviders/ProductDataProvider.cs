using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductDataProvider : EntityDataProvider<Product>
    {
        private const string INSERT_COMMAND = "INSERT INTO Product(ProductID, ProductName, Code, ProductIsActive) VALUES(@ProductID, @ProductName, @Code, @ProductIsActive);";
        private const string UPDATE_COMMAND = "UPDATE Product SET ProductID=@ProductID, ProductName=@ProductName, Code=@Code, ProductIsActive=@ProductIsActive WHERE ProductID=@ProductID;";
        private const string SELECT_COMMAND = "SELECT * FROM Product WHERE ProductID=@ProductID;";

        public override void Save(Product entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            int? id = null;
            if (entity.ProductID == null)
            {
                id = GetNewID(cmdCreater);
                cmd.CommandText = INSERT_COMMAND;
                cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = id;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            }

            cmd.Parameters.Add("@ProductName", MySqlDbType.VarChar).Value = entity.ProductName;
            cmd.Parameters.Add("@Code", MySqlDbType.VarChar).Value = entity.Code;
            cmd.Parameters.Add("@ProductIsActive", MySqlDbType.Bit).Value = entity.ProductIsActive;

            if (entity.ProductID == null)
            {
                cmd.ExecuteNonQuery();
                entity.ProductID = id;
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Product({0}) was not found in database.", entity.ProductID));
                }
            }
        }

        public override Product Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Product Load(DataRow row)
        {
            Product res = new Product();

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["ProductName"] is DBNull))
                res.ProductName = Convert.ToString(row["ProductName"]);
            if (!(row["Code"] is DBNull))
                res.Code = Convert.ToString(row["Code"]);
            if (!(row["ProductIsActive"] is DBNull))
                res.ProductIsActive = Convert.ToBoolean(row["ProductIsActive"]);

            return res;
        }

        private int? GetNewID(IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand q = cmdCreater.CreateCommand(@"
                select IfNull(max(ProductID), 0) from Product
            ");
            object res = q.ExecuteScalar();
            return Convert.ToInt32(res) + 1;
        }
    }
}
