using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderRecurringPlanStatusHistoryDataProvider : EntityDataProvider<OrderRecurringPlanStatusHistory>
    {
        private const string INSERT_COMMAND = "INSERT INTO OrderRecurringPlanStatusHistory(OrderRecurringPlanID, RecurringStatus, CreateDT) VALUES(@OrderRecurringPlanID, @RecurringStatus, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE OrderRecurringPlanStatusHistory SET OrderRecurringPlanID=@OrderRecurringPlanID, RecurringStatus=@RecurringStatus, CreateDT=@CreateDT WHERE OrderRecurringPlanStatusHistoryID=@OrderRecurringPlanStatusHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderRecurringPlanStatusHistory WHERE OrderRecurringPlanStatusHistoryID=@OrderRecurringPlanStatusHistoryID;";

        public override void Save(OrderRecurringPlanStatusHistory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderRecurringPlanStatusHistoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@OrderRecurringPlanStatusHistoryID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanStatusHistoryID;
            }

            cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;
            cmd.Parameters.Add("@RecurringStatus", MySqlDbType.Int32).Value = entity.RecurringStatus;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.OrderRecurringPlanStatusHistoryID == null)
            {
                entity.OrderRecurringPlanStatusHistoryID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderRecurringPlanStatusHistory({0}) was not found in database.", entity.OrderRecurringPlanStatusHistoryID));
                }
            }
        }

        public override OrderRecurringPlanStatusHistory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@OrderRecurringPlanStatusHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderRecurringPlanStatusHistory Load(DataRow row)
        {
            OrderRecurringPlanStatusHistory res = new OrderRecurringPlanStatusHistory();

            if (!(row["OrderRecurringPlanStatusHistoryID"] is DBNull))
                res.OrderRecurringPlanStatusHistoryID = Convert.ToInt64(row["OrderRecurringPlanStatusHistoryID"]);
            if (!(row["OrderRecurringPlanID"] is DBNull))
                res.OrderRecurringPlanID = Convert.ToInt64(row["OrderRecurringPlanID"]);
            if (!(row["RecurringStatus"] is DBNull))
                res.RecurringStatus = Convert.ToInt32(row["RecurringStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}