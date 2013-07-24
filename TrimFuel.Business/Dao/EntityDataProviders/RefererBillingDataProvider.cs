using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RefererBillingDataProvider : EntityDataProvider<RefererBilling>
    {
        private const string INSERT_COMMAND = "INSERT INTO RefererBilling(RefererID, BillingID) VALUES(@RefererID, @BillingID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RefererBilling SET RefererID=@RefererID, BillingID=@BillingID WHERE RefererBillingID=@RefererBillingID;";
        private const string SELECT_COMMAND = "SELECT * FROM RefererBilling WHERE RefererBillingID = @RefererBillingID;";

        public override void Save(RefererBilling entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RefererBillingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RefererBillingID", MySqlDbType.Int32).Value = entity.RefererBillingID;
            }

            cmd.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = entity.RefererID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;

            if (entity.RefererBillingID == null)
            {
                entity.RefererBillingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RefererBilling({0}) was not found in database.", entity.RefererBillingID));
                }
            }
        }

        public override RefererBilling Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RefererBillingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RefererBilling Load(DataRow row)
        {
            RefererBilling res = new RefererBilling();

            if (!(row["RefererBillingID"] is DBNull))
                res.RefererBillingID = Convert.ToInt32(row["RefererBillingID"]);
            if (!(row["RefererID"] is DBNull))
                res.RefererID = Convert.ToInt32(row["RefererID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);

            return res;
        }
    }
}
