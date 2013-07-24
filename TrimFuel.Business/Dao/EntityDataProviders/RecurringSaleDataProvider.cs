using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RecurringSaleDataProvider : EntityDataProvider<RecurringSale>
    {
        private OrderSaleDataProvider orderSaleDataProvider = new OrderSaleDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO RecurringSale(SaleID, OrderRecurringPlanID, RecurringCycle, ReAttempt) VALUES(@SaleID, @OrderRecurringPlanID, @RecurringCycle, @ReAttempt);";
        private const string UPDATE_COMMAND = "UPDATE RecurringSale SET OrderRecurringPlanID=@OrderRecurringPlanID, RecurringCycle=@RecurringCycle, ReAttempt=@ReAttempt WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderSale sl INNER JOIN RecurringSale rsl ON rsl.SaleID = sl.SaleID WHERE sl.SaleID=@SaleID;";

        public override void Save(RecurringSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;                
            }

            orderSaleDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;
            cmd.Parameters.Add("@RecurringCycle", MySqlDbType.Int32).Value = entity.RecurringCycle;
            cmd.Parameters.Add("@ReAttempt", MySqlDbType.Bit).Value = entity.ReAttempt;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("RecurringSale({0}) was not found in database.", entity.SaleID));
            }
        }

        public override RecurringSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RecurringSale Load(DataRow row)
        {
            OrderSale sale = (new OrderSaleDataProvider()).Load(row);

            RecurringSale res = new RecurringSale();
            res.FillFromOrderSale(sale);

            if (!(row["OrderRecurringPlanID"] is DBNull))
                res.OrderRecurringPlanID = Convert.ToInt64(row["OrderRecurringPlanID"]);
            if (!(row["RecurringCycle"] is DBNull))
                res.RecurringCycle = Convert.ToInt32(row["RecurringCycle"]);
            if (!(row["ReAttempt"] is DBNull))
                res.ReAttempt = Convert.ToBoolean(row["ReAttempt"]);

            return res;
        }
    }
}
