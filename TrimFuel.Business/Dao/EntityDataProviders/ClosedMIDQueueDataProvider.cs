using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ClosedMIDQueueDataProvider : EntityDataProvider<ClosedMIDQueue>
    {
        private const string INSERT_COMMAND = "INSERT INTO ClosedMIDQueue(ClosedMIDID, Queued, PaymentTypeID) VALUES(@ClosedMIDID, @Queued, @PaymentTypeID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ClosedMIDQueue SET ClosedMIDID=@ClosedMIDID, Queued=@Queued, PaymentTypeID=@PaymentTypeID WHERE ClosedMIDQueueID=@ClosedMIDQueueID;";
        private const string SELECT_COMMAND = "SELECT * FROM ClosedMIDQueue WHERE ClosedMIDQueueID=@ClosedMIDQueueID;";

        public override void Save(ClosedMIDQueue entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ClosedMIDQueueID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ClosedMIDQueueID", MySqlDbType.Int32).Value = entity.ClosedMIDQueueID;
            }

            cmd.Parameters.Add("@ClosedMIDID", MySqlDbType.Int32).Value = entity.ClosedMIDID;
            cmd.Parameters.Add("@Queued", MySqlDbType.Bit).Value = entity.Queued;
            cmd.Parameters.Add("@PaymentTypeID", MySqlDbType.Int32).Value = entity.PaymentTypeID;


            if (entity.ClosedMIDQueueID == null)
            {
                entity.ClosedMIDQueueID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ClosedMIDQueue({0}) was not found in database.", entity.ClosedMIDQueueID));
                }
            }
        }

        public override ClosedMIDQueue Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ClosedMIDQueueID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ClosedMIDQueue Load(DataRow row)
        {
            ClosedMIDQueue res = new ClosedMIDQueue();

            if (!(row["ClosedMIDQueueID"] is DBNull))
                res.ClosedMIDQueueID = Convert.ToInt32(row["ClosedMIDQueueID"]);
            if (!(row["ClosedMIDID"] is DBNull))
                res.ClosedMIDID = Convert.ToInt32(row["ClosedMIDID"]);
            if (!(row["Queued"] is DBNull))
                res.Queued = Convert.ToBoolean(row["Queued"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);

            return res;
        }
    }
}
