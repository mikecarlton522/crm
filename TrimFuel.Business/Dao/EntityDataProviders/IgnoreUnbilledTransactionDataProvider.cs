using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class IgnoreUnbilledTransactionDataProvider : EntityDataProvider<IgnoreUnbilledTransaction>
    {
        private const string INSERT_COMMAND = "INSERT INTO IgnoreUnbilledTransaction(BillingID, BillingSubscriptionID, OrderRecurringPlanID) VALUES(@BillingID, @BillingSubscriptionID, @OrderRecurringPlanID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE IgnoreUnbilledTransaction SET BillingID=@BillingID, BillingSubscriptionID=@BillingSubscriptionID, OrderRecurringPlanID=@OrderRecurringPlanID WHERE IgnoreUnbilledTransactionID=@IgnoreUnbilledTransactionID;";
        private const string SELECT_COMMAND = "SELECT * FROM IgnoreUnbilledTransaction WHERE IgnoreUnbilledTransactionID=@IgnoreUnbilledTransactionID;";

        public override void Save(IgnoreUnbilledTransaction entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.IgnoreUnbilledTransactionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IgnoreUnbilledTransactionID", MySqlDbType.Int32).Value = entity.IgnoreUnbilledTransactionID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;


            if (entity.IgnoreUnbilledTransactionID == null)
            {
                entity.IgnoreUnbilledTransactionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("IgnoreUnbilledTransaction({0}) was not found in database.", entity.IgnoreUnbilledTransactionID));
                }
            }
        }

        public override IgnoreUnbilledTransaction Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IgnoreUnbilledTransactionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override IgnoreUnbilledTransaction Load(DataRow row)
        {
            IgnoreUnbilledTransaction res = new IgnoreUnbilledTransaction();

            if (!(row["IgnoreUnbilledTransactionID"] is DBNull))
                res.IgnoreUnbilledTransactionID = Convert.ToInt32(row["IgnoreUnbilledTransactionID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["OrderRecurringPlanID"] is DBNull))
                res.OrderRecurringPlanID = Convert.ToInt64(row["OrderRecurringPlanID"]);

            return res;
        }
    }
}