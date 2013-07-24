using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderQueueNoteDataProvider : EntityDataProvider<OrderQueueNote>
    {
        private const string INSERT_COMMAND = "INSERT INTO OrderQueueNote(OrderID, Reason) VALUES(@OrderID, @Reason); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE OrderQueueNote SET OrderID=@OrderID, Reason=@Reason WHERE OrderID=@ID_OrderID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderQueueNote WHERE OrderQueueNoteID=@OrderQueueNoteID;";

        public override void Save(OrderQueueNote entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderQueueNoteID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ID_OrderID", MySqlDbType.Int64).Value = entity.OrderQueueNoteID.Value.OrderID;
            }

            cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = entity.OrderID;
            cmd.Parameters.Add("@Reason", MySqlDbType.VarChar).Value = entity.Reason;


            if (entity.OrderQueueNoteID == null)
            {
                cmd.ExecuteNonQuery();
                entity.OrderQueueNoteID = new OrderQueueNote.ID() { OrderID = entity.OrderID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderQueueNote({0}) was not found in database.", entity.OrderQueueNoteID.Value.OrderID));
                }
                else
                {
                    entity.OrderQueueNoteID = new OrderQueueNote.ID() { OrderID = entity.OrderID.Value };
                }
            }
        }

        public override OrderQueueNote Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ID_OrderID", MySqlDbType.Int64).Value = ((OrderQueueNote.ID?)key).Value.OrderID;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderQueueNote Load(DataRow row)
        {
            OrderQueueNote res = new OrderQueueNote();

            if (!(row["OrderID"] is DBNull))
                res.OrderID = Convert.ToInt64(row["OrderID"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);

            if (res.OrderID != null)
            {
                res.OrderQueueNoteID = new OrderQueueNote.ID()
                {
                    OrderID = res.OrderID.Value
                };
            }

            return res;
        }
    }
}