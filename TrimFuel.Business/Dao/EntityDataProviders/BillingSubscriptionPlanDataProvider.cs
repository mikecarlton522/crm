using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingSubscriptionPlanDataProvider : EntityDataProvider<BillingSubscriptionPlan>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingSubscriptionPlan(BillingSubscriptionID, LastItemID, LastItemDate, NextItemID, NextItemDate, SubscriptionPlanID, CreateDT, IsActive, OrderRecurringPlanID) VALUES(@BillingSubscriptionID, @LastItemID, @LastItemDate, @NextItemID, @NextItemDate, @SubscriptionPlanID, @CreateDT, @IsActive, @OrderRecurringPlanID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingSubscriptionPlan SET BillingSubscriptionID=@BillingSubscriptionID, LastItemID=@LastItemID, LastItemDate=@LastItemDate, NextItemID=@NextItemID, NextItemDate=@NextItemDate, SubscriptionPlanID=@SubscriptionPlanID, CreateDT=@CreateDT, IsActive=@IsActive, OrderRecurringPlanID=@OrderRecurringPlanID WHERE BillingSubscriptionPlanID=@BillingSubscriptionPlanID;";
        private const string SELECT_COMMAND = "SELECT * FROM BillingSubscriptionPlan WHERE BillingSubscriptionPlanID=@BillingSubscriptionPlanID;";

        public override void Save(BillingSubscriptionPlan entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingSubscriptionPlanID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingSubscriptionPlanID", MySqlDbType.Int32).Value = entity.BillingSubscriptionPlanID;
            }
            
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;
            cmd.Parameters.Add("@LastItemID", MySqlDbType.Int32).Value = entity.LastItemID;
            cmd.Parameters.Add("@LastItemDate", MySqlDbType.Timestamp).Value = entity.LastItemDate;
            cmd.Parameters.Add("@NextItemID", MySqlDbType.Int32).Value = entity.NextItemID;
            cmd.Parameters.Add("@NextItemDate", MySqlDbType.Timestamp).Value = entity.NextItemDate;
            cmd.Parameters.Add("@SubscriptionPlanID", MySqlDbType.Int32).Value = entity.SubscriptionPlanID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@IsActive", MySqlDbType.Bit).Value = entity.IsActive;

            if (entity.BillingSubscriptionPlanID == null)
            {
                entity.BillingSubscriptionPlanID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingSubscriptionPlan({0}) was not found in database.", entity.BillingSubscriptionPlanID));
                }
            }
        }

        public override BillingSubscriptionPlan Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingSubscriptionPlanID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingSubscriptionPlan Load(DataRow row)
        {
            BillingSubscriptionPlan res = new BillingSubscriptionPlan();

            if (!(row["BillingSubscriptionPlanID"] is DBNull))
                res.BillingSubscriptionPlanID = Convert.ToInt32(row["BillingSubscriptionPlanID"]);
            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["OrderRecurringPlanID"] is DBNull))
                res.OrderRecurringPlanID = Convert.ToInt64(row["OrderRecurringPlanID"]);
            if (!(row["LastItemID"] is DBNull))
                res.LastItemID = Convert.ToInt32(row["LastItemID"]);
            if (!(row["LastItemDate"] is DBNull))
                res.LastItemDate = Convert.ToDateTime(row["LastItemDate"]);
            if (!(row["NextItemID"] is DBNull))
                res.NextItemID = Convert.ToInt32(row["NextItemID"]);
            if (!(row["NextItemDate"] is DBNull))
                res.NextItemDate = Convert.ToDateTime(row["NextItemDate"]);
            if (!(row["SubscriptionPlanID"] is DBNull))
                res.SubscriptionPlanID = Convert.ToInt32(row["SubscriptionPlanID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["IsActive"] is DBNull))
                res.IsActive = Convert.ToBoolean(row["IsActive"]);

            return res;
        }
    }
}
