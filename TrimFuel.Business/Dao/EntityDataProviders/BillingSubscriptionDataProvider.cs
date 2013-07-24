using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingSubscriptionDataProvider : EntityDataProvider<BillingSubscription>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingSubscription(BillingID, SubscriptionID, CreateDT, StatusTID, LastBillDate, NextBillDate, CustomerReferenceNumber) VALUES(@BillingID, @SubscriptionID, @CreateDT, @StatusTID, @LastBillDate, @NextBillDate, @CustomerReferenceNumber); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingSubscription SET BillingID=@BillingID, SubscriptionID=@SubscriptionID, CreateDT=@CreateDT, StatusTID=@StatusTID, LastBillDate=@LastBillDate, NextBillDate=@NextBillDate, CustomerReferenceNumber=@CustomerReferenceNumber WHERE BillingSubscriptionID=@BillingSubscriptionID;";
        private const string SELECT_COMMAND = "SELECT * FROM BillingSubscription WHERE BillingSubscriptionID=@BillingSubscriptionID;";

        public override void Save(BillingSubscription entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingSubscriptionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = entity.SubscriptionID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@StatusTID", MySqlDbType.Int32).Value = entity.StatusTID;
            cmd.Parameters.Add("@LastBillDate", MySqlDbType.Timestamp).Value = entity.LastBillDate;
            cmd.Parameters.Add("@NextBillDate", MySqlDbType.Timestamp).Value = entity.NextBillDate;
            //cmd.Parameters.Add("@SKU", MySqlDbType.VarChar).Value = entity.SKU;
            cmd.Parameters.Add("@CustomerReferenceNumber", MySqlDbType.VarChar).Value = entity.CustomerReferenceNumber;

            if (entity.BillingSubscriptionID == null)
            {
                entity.BillingSubscriptionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingSubscription({0}) was not found in database.", entity.BillingSubscriptionID));
                }
            }
        }

        public override BillingSubscription Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingSubscription Load(DataRow row)
        {
            BillingSubscription res = new BillingSubscription();

            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["StatusTID"] is DBNull))
                res.StatusTID = Convert.ToInt32(row["StatusTID"]);
            if (!(row["LastBillDate"] is DBNull))
                res.LastBillDate = Convert.ToDateTime(row["LastBillDate"]);
            if (!(row["NextBillDate"] is DBNull))
                res.NextBillDate = Convert.ToDateTime(row["NextBillDate"]);
            //if (!(row["SKU"] is DBNull))
            //    res.SKU = Convert.ToString(row["SKU"]);
            if (!(row["CustomerReferenceNumber"] is DBNull))
                res.CustomerReferenceNumber = Convert.ToString(row["CustomerReferenceNumber"]);

            return res;
        }
    }
}
