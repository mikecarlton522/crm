using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingSubscriptionRebillDiscountDataProvider : EntityDataProvider<BillingSubscriptionRebillDiscount>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingSubscriptionRebillDiscount(DiscountTypeID, DiscountValue, BillingSubscriptionID) VALUES(@DiscountTypeID, @DiscountValue, @BillingSubscriptionID);";
        private const string UPDATE_COMMAND = "UPDATE BillingSubscriptionRebillDiscount SET DiscountTypeID=@DiscountTypeID, DiscountValue=@DiscountValue WHERE BillingSubscriptionID=@BillingSubscriptionID";
        private const string SELECT_COMMAND = "SELECT * FROM BillingSubscriptionRebillDiscount WHERE BillingSubscriptionID=@BillingSubscriptionID;";

        public override void Save(BillingSubscriptionRebillDiscount entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var existed = Load(entity.BillingSubscriptionID, cmdCreater);
            if (existed == null)
                cmd.CommandText = INSERT_COMMAND;
            else
                cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@DiscountTypeID", MySqlDbType.Int32).Value = entity.DiscountTypeID;
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@DiscountValue", MySqlDbType.Decimal).Value = entity.DiscountValue;


            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("BillingSubscriptionRebillDiscount({0}) was not found in database.", entity.BillingSubscriptionID));
            }
        }

        public override BillingSubscriptionRebillDiscount Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingSubscriptionRebillDiscount Load(DataRow row)
        {
            BillingSubscriptionRebillDiscount res = new BillingSubscriptionRebillDiscount();

            if (!(row["DiscountTypeID"] is DBNull))
                res.DiscountTypeID = Convert.ToInt32(row["DiscountTypeID"]);
            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["DiscountValue"] is DBNull))
                res.DiscountValue = Convert.ToDecimal(row["DiscountValue"]);

            return res;
        }
    }
}
