using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderDataProvider : EntityDataProvider<Order>
    {
        private const string INSERT_COMMAND = "INSERT INTO Orders(CampaignID, Affiliate, SubAffiliate, OrderAuthor, IP, URL, BillingID, ProductID, Scrub, RefererID, CouponCode, OrderStatus, CreateDT) VALUES(@CampaignID, @Affiliate, @SubAffiliate, @OrderAuthor, @IP, @URL, @BillingID, @ProductID, @Scrub, @RefererID, @CouponCode, @OrderStatus, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Orders SET CampaignID=@CampaignID, Affiliate=@Affiliate, SubAffiliate=@SubAffiliate, OrderAuthor=@OrderAuthor, IP=@IP, URL=@URL, BillingID=@BillingID, ProductID=@ProductID, Scrub=@Scrub, RefererID=@RefererID, CouponCode=@CouponCode, OrderStatus=@OrderStatus, CreateDT=@CreateDT WHERE OrderID=@OrderID;";
        private const string SELECT_COMMAND = "SELECT * FROM Orders WHERE OrderID=@OrderID;";

        public override void Save(Order entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = entity.OrderID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = entity.SubAffiliate;
            cmd.Parameters.Add("@OrderAuthor", MySqlDbType.VarChar).Value = entity.OrderAuthor;
            cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = entity.IP;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URL;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Scrub", MySqlDbType.Bit).Value = entity.Scrub;
            cmd.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = entity.RefererID;
            cmd.Parameters.Add("@CouponCode", MySqlDbType.VarChar).Value = entity.CouponCode;
            cmd.Parameters.Add("@OrderStatus", MySqlDbType.Int32).Value = entity.OrderStatus;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.OrderID == null)
            {
                entity.OrderID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Order({0}) was not found in database.", entity.OrderID));
                }
            }
        }

        public override Order Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Order Load(DataRow row)
        {
            Order res = new Order();

            if (!(row["OrderID"] is DBNull))
                res.OrderID = Convert.ToInt64(row["OrderID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["SubAffiliate"] is DBNull))
                res.SubAffiliate = Convert.ToString(row["SubAffiliate"]);
            if (!(row["OrderAuthor"] is DBNull))
                res.OrderAuthor = Convert.ToString(row["OrderAuthor"]);
            if (!(row["IP"] is DBNull))
                res.IP = Convert.ToString(row["IP"]);
            if (!(row["URL"] is DBNull))
                res.URL = Convert.ToString(row["URL"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Scrub"] is DBNull))
                res.Scrub = Convert.ToBoolean(row["Scrub"]);
            if (!(row["RefererID"] is DBNull))
                res.RefererID = Convert.ToInt32(row["RefererID"]);
            if (!(row["CouponCode"] is DBNull))
                res.CouponCode = Convert.ToString(row["CouponCode"]);
            if (!(row["OrderStatus"] is DBNull))
                res.OrderStatus = Convert.ToInt32(row["OrderStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
