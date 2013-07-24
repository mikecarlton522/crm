using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderAutoProcessingDataProvider : EntityDataProvider<OrderAutoProcessing>
    {
        private const string INSERT_COMMAND = "INSERT INTO OrderAutoProcessing(OrderID, Completed, ScheduleDT, CompleteDT) VALUES(@OrderID, @Completed, @ScheduleDT, @CompleteDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE OrderAutoProcessing SET OrderID=@OrderID, Completed=@Completed, ScheduleDT=@ScheduleDT, CompleteDT=@CompleteDT WHERE OrderID=@ID_OrderID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderAutoProcessing WHERE ID_OrderID=@ID_OrderID;";

        public override void Save(OrderAutoProcessing entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderAutoProcessingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ID_OrderID", MySqlDbType.Int64).Value = entity.OrderAutoProcessingID.Value.OrderID;
            }

            cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = entity.OrderID;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;
            cmd.Parameters.Add("@ScheduleDT", MySqlDbType.Timestamp).Value = entity.ScheduleDT;
            cmd.Parameters.Add("@CompleteDT", MySqlDbType.Timestamp).Value = entity.CompleteDT;


            if (entity.OrderAutoProcessingID == null)
            {
                cmd.ExecuteNonQuery();
                entity.OrderAutoProcessingID = new OrderAutoProcessing.ID() { OrderID = entity.OrderID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderAutoProcessing({0}) was not found in database.", entity.OrderAutoProcessingID.Value.OrderID));
                }
                else
                {
                    entity.OrderAutoProcessingID = new OrderAutoProcessing.ID() { OrderID = entity.OrderID.Value };
                }
            }
        }

        public override OrderAutoProcessing Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ID_OrderID", MySqlDbType.Int64).Value = ((OrderAutoProcessing.ID?)key).Value.OrderID;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderAutoProcessing Load(DataRow row)
        {
            OrderAutoProcessing res = new OrderAutoProcessing();

            if (!(row["OrderID"] is DBNull))
                res.OrderID = Convert.ToInt64(row["OrderID"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["ScheduleDT"] is DBNull))
                res.ScheduleDT = Convert.ToDateTime(row["ScheduleDT"]);
            if (!(row["CompleteDT"] is DBNull))
                res.CompleteDT = Convert.ToDateTime(row["CompleteDT"]);

            if (res.OrderID != null)
            {
                res.OrderAutoProcessingID = new OrderAutoProcessing.ID()
                {
                    OrderID = res.OrderID.Value
                };
            }

            return res;
        }
    }
}
