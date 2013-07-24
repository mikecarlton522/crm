using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderRecurringPlanDataProvider : EntityDataProvider<OrderRecurringPlan>
    {
        private const string INSERT_COMMAND = "INSERT INTO OrderRecurringPlan(SaleID, RecurringPlanID, TrialInterim, RecurringStatus, StartDT, NextCycleDT, DiscountValue, DiscountTypeID) VALUES(@SaleID, @RecurringPlanID, @TrialInterim, @RecurringStatus, @StartDT, @NextCycleDT, @DiscountValue, @DiscountTypeID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE OrderRecurringPlan SET SaleID=@SaleID, RecurringPlanID=@RecurringPlanID, TrialInterim=@TrialInterim, RecurringStatus=@RecurringStatus, StartDT=@StartDT, NextCycleDT=@NextCycleDT, DiscountValue=@DiscountValue, DiscountTypeID=@DiscountTypeID WHERE OrderRecurringPlanID=@OrderRecurringPlanID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderRecurringPlan WHERE OrderRecurringPlanID=@OrderRecurringPlanID;";

        public override void Save(OrderRecurringPlan entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderRecurringPlanID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RecurringPlanID", MySqlDbType.Int32).Value = entity.RecurringPlanID;
            cmd.Parameters.Add("@TrialInterim", MySqlDbType.Int32).Value = entity.TrialInterim;
            cmd.Parameters.Add("@RecurringStatus", MySqlDbType.Int32).Value = entity.RecurringStatus;
            cmd.Parameters.Add("@DiscountTypeID", MySqlDbType.Int32).Value = entity.DiscountTypeID;
            cmd.Parameters.Add("@DiscountValue", MySqlDbType.Decimal).Value = entity.DiscountValue;
            cmd.Parameters.Add("@StartDT", MySqlDbType.Timestamp).Value = entity.StartDT;
            cmd.Parameters.Add("@NextCycleDT", MySqlDbType.Timestamp).Value = entity.NextCycleDT;


            if (entity.OrderRecurringPlanID == null)
            {
                entity.OrderRecurringPlanID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderRecurringPlan({0}) was not found in database.", entity.OrderRecurringPlanID));
                }
            }
        }

        public override OrderRecurringPlan Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderRecurringPlan Load(DataRow row)
        {
            OrderRecurringPlan res = new OrderRecurringPlan();

            if (!(row["OrderRecurringPlanID"] is DBNull))
                res.OrderRecurringPlanID = Convert.ToInt64(row["OrderRecurringPlanID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["RecurringPlanID"] is DBNull))
                res.RecurringPlanID = Convert.ToInt32(row["RecurringPlanID"]);
            if (!(row["TrialInterim"] is DBNull))
                res.TrialInterim = Convert.ToInt32(row["TrialInterim"]);
            if (!(row["RecurringStatus"] is DBNull))
                res.RecurringStatus = Convert.ToInt32(row["RecurringStatus"]);
            if (!(row["StartDT"] is DBNull))
                res.StartDT = Convert.ToDateTime(row["StartDT"]);
            if (!(row["NextCycleDT"] is DBNull))
                res.NextCycleDT = Convert.ToDateTime(row["NextCycleDT"]);
            if (!(row["DiscountTypeID"] is DBNull))
                res.DiscountTypeID = Convert.ToInt32(row["DiscountTypeID"]);
            if (!(row["DiscountValue"] is DBNull))
                res.DiscountValue = Convert.ToDecimal(row["DiscountValue"]);

            return res;
        }
    }
}
