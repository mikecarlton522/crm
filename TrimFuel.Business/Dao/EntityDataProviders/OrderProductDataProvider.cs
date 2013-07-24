using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderProductDataProvider : EntityDataProvider<OrderProduct>
    {
        private const string INSERT_COMMAND = "INSERT INTO OrderProduct(SaleID, ProductSKU, Quantity) VALUES(@SaleID, @ProductSKU, @Quantity); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE OrderProduct SET SaleID=@SaleID, ProductSKU=@ProductSKU, Quantity=@Quantity WHERE OrderProductID=@OrderProductID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderProduct WHERE OrderProductID=@OrderProductID;";

        public override void Save(OrderProduct entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderProductID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@OrderProductID", MySqlDbType.Int64).Value = entity.OrderProductID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = entity.ProductSKU;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;


            if (entity.OrderProductID == null)
            {
                entity.OrderProductID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderProduct({0}) was not found in database.", entity.OrderProductID));
                }
            }
        }

        public override OrderProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@OrderProductID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderProduct Load(DataRow row)
        {
            OrderProduct res = new OrderProduct();

            if (!(row["OrderProductID"] is DBNull))
                res.OrderProductID = Convert.ToInt64(row["OrderProductID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["ProductSKU"] is DBNull))
                res.ProductSKU = Convert.ToString(row["ProductSKU"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
