using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class InventoryDataProvider : EntityDataProvider<Inventory>
    {
        private const string INSERT_COMMAND =
            "INSERT INTO Inventory (SKU, Product, InStock, Cost, RetailPrice, InventoryType) VALUES (@Sku, @Product, @InStock, @Cost, @RetailPrice, @InventoryType); SELECT @@IDENTITY;";

        private const string UPDATE_COMMAND =
            "UPDATE Inventory SET SKU=@SKU, Product=@Product, InStock=@InStock, Cost=@Cost, RetailPrice=@RetailPrice, InventoryType=@InventoryType WHERE InventoryID=@InventoryID;";

        private const string SELECT_COMMAND = "SELECT * FROM Inventory WHERE InventoryID=@InventoryID;";

        public override void Save(Inventory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.InventoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = entity.InventoryID;
            }

            cmd.Parameters.Add("@Sku", MySqlDbType.String).Value = entity.SKU;
            cmd.Parameters.Add("@Product", MySqlDbType.String).Value = entity.Product;
            cmd.Parameters.Add("@InStock", MySqlDbType.Int32).Value = entity.InStock;
            cmd.Parameters.Add("@Cost", MySqlDbType.Decimal).Value = entity.Costs;
            cmd.Parameters.Add("@RetailPrice", MySqlDbType.Decimal).Value = entity.RetailPrice;
            cmd.Parameters.Add("@InventoryType", MySqlDbType.Int32).Value = entity.InventoryType;


            if (entity.InventoryID == null)
            {
                entity.InventoryID = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Inventory({0}) was not found in database.", entity.InventoryID));
                }
            }
        }

        public override Inventory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Inventory Load(DataRow row)
        {
            Inventory res = new Inventory();

            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);
            if (!(row["SKU"] is DBNull))
                res.SKU = Convert.ToString(row["SKU"]);
            if (!(row["Product"] is DBNull))
                res.Product = Convert.ToString(row["Product"]);
            if (!(row["InStock"] is DBNull))
                res.InStock = Convert.ToInt32(row["InStock"]);
            if (!(row["Cost"] is DBNull))
                res.Costs = Convert.ToDecimal(row["Cost"]);
            if (!(row["RetailPrice"] is DBNull))
                res.RetailPrice = Convert.ToDecimal(row["RetailPrice"]);
            if (!(row["InventoryType"] is DBNull))
                res.InventoryType = Convert.ToInt32(row["InventoryType"]);

            return res;
        }
    }
}