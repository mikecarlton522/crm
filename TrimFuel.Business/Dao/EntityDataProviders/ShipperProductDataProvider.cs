using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipperProductDataProvider : EntityDataProvider<ShipperProduct>
    {
        private const string INSERT_COMMAND = "INSERT INTO ShipperProduct(ShipperID, NeedConfirm, ProductID) VALUES(@ShipperID, @NeedConfirm, @ProductID)";
        private const string UPDATE_COMMAND = "UPDATE ShipperProduct SET ShipperID=@ShipperID, NeedConfirm=@NeedConfirm WHERE ProductID=@ProductID;";
        private const string SELECT_COMMAND = "SELECT * FROM ShipperProduct WHERE ProductID=@ProductID;";

        public override void Save(ShipperProduct entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            var itemInDB = Load(cmd).SingleOrDefault();

            cmd = cmdCreater.CreateCommand();
            if (itemInDB == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int64).Value = entity.ProductID;
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int64).Value = entity.ShipperID;
            cmd.Parameters.Add("@NeedConfirm", MySqlDbType.Bit).Value = entity.NeedConfirm;

            if (itemInDB == null)
            {
                cmd.ExecuteScalar();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductID ({0}) was not found in database.", entity.ProductID));
                }
            }
        }

        public override ShipperProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ShipperProduct Load(System.Data.DataRow row)
        {
            ShipperProduct res = new ShipperProduct();

            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["NeedConfirm"] is DBNull))
                res.NeedConfirm = Convert.ToBoolean(row["NeedConfirm"]);

            return res;
        }
    }
}
