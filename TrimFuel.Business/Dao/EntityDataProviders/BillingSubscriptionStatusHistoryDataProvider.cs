using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingSubscriptionStatusHistoryDataProvider : EntityDataProvider<BillingSubscriptionStatusHistory>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingSubscriptionStatusHistory(BillingSubscriptionID, StatusTID, CreateDT) VALUES(@BillingSubscriptionID, @StatusTID, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingSubscriptionStatusHistory SET BillingSubscriptionID=@BillingSubscriptionID, StatusTID=@StatusTID, CreateDT=@CreateDT WHERE BillingSubscriptionStatusHistoryID=@BillingSubscriptionStatusHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM BillingSubscriptionStatusHistory WHERE BillingSubscriptionStatusHistoryID=@BillingSubscriptionStatusHistoryID;";

        public override void Save(BillingSubscriptionStatusHistory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingSubscriptionStatusHistoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingSubscriptionStatusHistoryID", MySqlDbType.Int32).Value = entity.BillingSubscriptionStatusHistoryID;
            }

            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@StatusTID", MySqlDbType.Int32).Value = entity.StatusTID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.BillingSubscriptionStatusHistoryID == null)
            {
                entity.BillingSubscriptionStatusHistoryID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingSubscriptionStatusHistory({0}) was not found in database.", entity.BillingSubscriptionStatusHistoryID));
                }
            }
        }

        public override BillingSubscriptionStatusHistory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingSubscriptionStatusHistoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingSubscriptionStatusHistory Load(DataRow row)
        {
            BillingSubscriptionStatusHistory res = new BillingSubscriptionStatusHistory();

            if (!(row["BillingSubscriptionStatusHistoryID"] is DBNull))
                res.BillingSubscriptionStatusHistoryID = Convert.ToInt32(row["BillingSubscriptionStatusHistoryID"]);
            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["StatusTID"] is DBNull))
                res.StatusTID = Convert.ToInt32(row["StatusTID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
