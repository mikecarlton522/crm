using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class InventorySKUDataProvider : EntityDataProvider<InventorySKU>
    {
        private const string INSERT_COMMAND = "INSERT INTO InventorySKU(ProductSKU, ShipperID, InventorySKU, InStock, Cost) VALUES(@ProductSKU, @ShipperID, @InventorySKU, @InStock, @Cost); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE InventorySKU SET ProductSKU=@ProductSKU, ShipperID=@ShipperID, InventorySKU=@InventorySKU, InStock=@InStock, Cost=@Cost WHERE ProductSKU=@IDProductSKU and ShipperID=@IDShipperID;";
        private const string SELECT_COMMAND = "SELECT * FROM InventorySKU WHERE ProductSKU=@IDProductSKU and ShipperID=@IDShipperID;";

        public override void Save(InventorySKU entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.InventorySKUID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDProductSKU", MySqlDbType.VarChar).Value = entity.InventorySKUID.Value.ProductSKU;
                cmd.Parameters.Add("@IDShipperID", MySqlDbType.Int32).Value = entity.InventorySKUID.Value.ShipperID;
            }

            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = entity.ProductSKU;
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = entity.ShipperID;
            cmd.Parameters.Add("@InventorySKU", MySqlDbType.VarChar).Value = entity.InventorySKU_;
            cmd.Parameters.Add("@InStock", MySqlDbType.Int32).Value = entity.InStock;
            cmd.Parameters.Add("@Cost", MySqlDbType.Decimal).Value = entity.Cost;


            if (entity.InventorySKUID == null)
            {
                cmd.ExecuteNonQuery();
                entity.InventorySKUID = new InventorySKU.ID() { ProductSKU = entity.ProductSKU, ShipperID = entity.ShipperID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("InventorySKU({0},{1}) was not found in database.", entity.InventorySKUID.Value.ProductSKU, entity.InventorySKUID.Value.ShipperID));
                }
                else
                {
                    entity.InventorySKUID = new InventorySKU.ID() { ProductSKU = entity.ProductSKU, ShipperID = entity.ShipperID.Value };
                }
            }
        }

        public override InventorySKU Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDProductSKU", MySqlDbType.VarChar).Value = ((InventorySKU.ID?)key).Value.ProductSKU;
            cmd.Parameters.Add("@IDShipperID", MySqlDbType.Int32).Value = ((InventorySKU.ID?)key).Value.ShipperID;

            return Load(cmd).FirstOrDefault();
        }

        public override InventorySKU Load(DataRow row)
        {
            InventorySKU res = new InventorySKU();

            if (!(row["ProductSKU"] is DBNull))
                res.ProductSKU = Convert.ToString(row["ProductSKU"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["InventorySKU"] is DBNull))
                res.InventorySKU_ = Convert.ToString(row["InventorySKU"]);
            if (!(row["InStock"] is DBNull))
                res.InStock = Convert.ToInt32(row["InStock"]);
            if (!(row["Cost"] is DBNull))
                res.Cost = Convert.ToDecimal(row["Cost"]);

            if (res.ProductSKU != null && res.ShipperID != null)
            {
                res.InventorySKUID = new InventorySKU.ID() { ProductSKU = res.ProductSKU, ShipperID = res.ShipperID.Value };
            }

            return res;
        }
    }
}