using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderSaleDataProvider : EntityDataProvider<OrderSale>
    {
        private SaleDataProvider saleDataProvider = new SaleDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO OrderSale(SaleID, SaleName, SaleType, Quantity, OrderID, PurePrice, SaleStatus, InvoiceID, CreateDT, ProcessDT) VALUES(@SaleID, @SaleName, @SaleType, @Quantity, @OrderID, @PurePrice, @SaleStatus, @InvoiceID, @CreateDT, @ProcessDT);";
        private const string UPDATE_COMMAND = "UPDATE OrderSale SET SaleName=@SaleName, SaleType=@SaleType, Quantity=@Quantity, OrderID=@OrderID, PurePrice=@PurePrice, SaleStatus=@SaleStatus, InvoiceID=@InvoiceID, CreateDT=@CreateDT, ProcessDT=@ProcessDT WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderSale WHERE SaleID=@SaleID;";

        public override void Save(OrderSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();
            Sale sale = null;

            if (entity.SaleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
                //Create Sale record
                sale = new Sale()
                {
                    CreateDT = entity.CreateDT,
                    NotShip = true,
                    SaleTypeID = SaleTypeEnum.OrderSale,
                    TrackingNumber = null
                };
                saleDataProvider.Save(sale, cmdCreater);
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;                
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID ?? sale.SaleID;
            cmd.Parameters.Add("@SaleName", MySqlDbType.VarChar).Value = entity.SaleName;
            cmd.Parameters.Add("@SaleType", MySqlDbType.Int32).Value = entity.SaleType;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = entity.OrderID;
            cmd.Parameters.Add("@PurePrice", MySqlDbType.Decimal).Value = entity.PurePrice;
            cmd.Parameters.Add("@SaleStatus", MySqlDbType.Int32).Value = entity.SaleStatus;
            cmd.Parameters.Add("@InvoiceID", MySqlDbType.Int64).Value = entity.InvoiceID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@ProcessDT", MySqlDbType.Timestamp).Value = entity.ProcessDT;


            if (entity.SaleID == null)
            {
                cmd.ExecuteNonQuery();
                entity.SaleID = sale.SaleID; //Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderSale({0}) was not found in database.", entity.SaleID));
                }
            }
        }

        public override OrderSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderSale Load(DataRow row)
        {
            OrderSale res = new OrderSale();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["SaleName"] is DBNull))
                res.SaleName = Convert.ToString(row["SaleName"]);
            if (!(row["SaleType"] is DBNull))
                res.SaleType = Convert.ToInt32(row["SaleType"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["OrderID"] is DBNull))
                res.OrderID = Convert.ToInt64(row["OrderID"]);
            if (!(row["PurePrice"] is DBNull))
                res.PurePrice = Convert.ToDecimal(row["PurePrice"]);
            if (!(row["SaleStatus"] is DBNull))
                res.SaleStatus = Convert.ToInt32(row["SaleStatus"]);
            if (!(row["InvoiceID"] is DBNull))
                res.InvoiceID = Convert.ToInt64(row["InvoiceID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["ProcessDT"] is DBNull))
                res.ProcessDT = Convert.ToDateTime(row["ProcessDT"]);

            return res;
        }
    }
}