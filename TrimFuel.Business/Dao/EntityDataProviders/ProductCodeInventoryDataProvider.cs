using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    class ProductCodeInventoryDataProvider : EntityDataProvider<ProductCodeInventory>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductCodeInventory(ProductCodeID, InventoryID, Quantity) VALUES(@ProductCodeID, @InventoryID, @Quantity); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ABFRecord SET ProductCodeID=@ProductCodeID, InventoryID=@InventoryID, Quantity=@Quantity WHERE ProductCodeInventoryID=@ProductCodeInventoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductCodeInventory WHERE ProductCodeInventoryID=@ProductCodeInventoryID;";

        public override void Save(ProductCodeInventory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductCodeInventoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductCodeInventoryID", MySqlDbType.Int32).Value = entity.ProductCodeInventoryID;
            }

            cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int64).Value = entity.ProductCodeID;
            cmd.Parameters.Add("@InventoryID", MySqlDbType.Int64).Value = entity.InventoryID;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int64).Value = entity.Quantity;

            if (entity.ProductCodeInventoryID == null)
            {
                entity.ProductCodeInventoryID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductCodeInventory ({0}) was not found in database.", entity.ProductCodeInventoryID));
                }
            }
        }

        public override ProductCodeInventory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductCodeInventoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductCodeInventory Load(System.Data.DataRow row)
        {
            ProductCodeInventory res = new ProductCodeInventory();

            if (!(row["ProductCodeInventoryID"] is DBNull))
                res.ProductCodeInventoryID = Convert.ToInt32(row["ProductCodeInventoryID"]);
            if (!(row["ProductCodeID"] is DBNull))
                res.ProductCodeID = Convert.ToInt32(row["ProductCodeID"]);
            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
