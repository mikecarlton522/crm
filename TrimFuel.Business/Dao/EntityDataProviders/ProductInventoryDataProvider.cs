using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductInventoryDataProvider : EntityDataProvider<ProductInventory>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductInventory(ProductID, InventoryID) VALUES(@ProductID, @InventoryID);";
        private const string UPDATE_COMMAND = "UPDATE ProductInventory SET ProductID=@ProductID, InventoryID=@InventoryID WHERE ProductID=@ProductID and InventoryID=@InventoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductInventory WHERE ProductID=@ProductID and InventoryID=@InventoryID;";

        public override void Save(ProductInventory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductInventoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductInventoryID.Value.ProductID;
                cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = entity.ProductInventoryID.Value.InventoryID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = entity.InventoryID;

            cmd.ExecuteNonQuery();
            entity.ProductInventoryID = new ProductInventory.ID() { InventoryID = entity.InventoryID, ProductID = entity.ProductID };
        }

        public override ProductInventory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = ((ProductInventory.ID?)key).Value.InventoryID;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = ((ProductInventory.ID?)key).Value.ProductID;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductInventory Load(DataRow row)
        {
            ProductInventory res = new ProductInventory();

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);

            res.ProductInventoryID = new ProductInventory.ID() { InventoryID = res.InventoryID, ProductID = res.ProductID };

            return res;
        }
    }
}
