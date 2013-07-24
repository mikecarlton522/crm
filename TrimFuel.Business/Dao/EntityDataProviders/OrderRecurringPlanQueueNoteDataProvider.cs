using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class OrderRecurringPlanQueueNoteDataProvider : EntityDataProvider<OrderRecurringPlanQueueNote>
    {
        private const string INSERT_COMMAND = "INSERT INTO OrderRecurringPlanQueueNote(OrderRecurringPlanID, Reason, Amount, MerchantAccountID, CreateDT) VALUES(@OrderRecurringPlanID, @Reason, @Amount, @MerchantAccountID, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE OrderRecurringPlanQueueNote SET OrderRecurringPlanID=@OrderRecurringPlanID, Reason=@Reason, Amount=@Amount, MerchantAccountID=@MerchantAccountID, CreateDT=@CreateDT WHERE OrderRecurringPlanID=@IDOrderRecurringPlanID;";
        private const string SELECT_COMMAND = "SELECT * FROM OrderRecurringPlanQueueNote WHERE OrderRecurringPlanID=@IDOrderRecurringPlanID;";

        public override void Save(OrderRecurringPlanQueueNote entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.OrderRecurringPlanQueueNoteID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDOrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanQueueNoteID.Value.OrderRecirringPlanID;
            }

            cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;
            cmd.Parameters.Add("@Reason", MySqlDbType.VarChar).Value = entity.Reason;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@MerchantAccountID", MySqlDbType.Int32).Value = entity.MerchantAccountID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.OrderRecurringPlanQueueNoteID == null)
            {
                cmd.ExecuteNonQuery();
                entity.OrderRecurringPlanQueueNoteID = new OrderRecurringPlanQueueNote.ID() { OrderRecirringPlanID = entity.OrderRecurringPlanID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("OrderRecurringPlanQueueNote({0}) was not found in database.", entity.OrderRecurringPlanQueueNoteID));
                }
                else
                {
                    entity.OrderRecurringPlanQueueNoteID = new OrderRecurringPlanQueueNote.ID() { OrderRecirringPlanID = entity.OrderRecurringPlanID.Value };
                }
            }
        }

        public override OrderRecurringPlanQueueNote Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDOrderRecurringPlanID", MySqlDbType.Int64).Value = ((OrderRecurringPlanQueueNote.ID?)key).Value.OrderRecirringPlanID;

            return Load(cmd).FirstOrDefault();
        }

        public override OrderRecurringPlanQueueNote Load(DataRow row)
        {
            OrderRecurringPlanQueueNote res = new OrderRecurringPlanQueueNote();

            if (!(row["OrderRecurringPlanID"] is DBNull))
                res.OrderRecurringPlanID = Convert.ToInt64(row["OrderRecurringPlanID"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (res.OrderRecurringPlanID != null)
            {
                res.OrderRecurringPlanQueueNoteID = new OrderRecurringPlanQueueNote.ID() { OrderRecirringPlanID = res.OrderRecurringPlanID.Value };
            }

            return res;
        }
    }
}
