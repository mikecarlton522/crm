using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingBatchEmailDataProvider : EntityDataProvider<BillingBatchEmail>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingBatchEmail(BillingBatchEmailTypeID, BillingID, EmailID) VALUES(@BillingBatchEmailTypeID, @BillingID, @EmailID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingBatchEmail SET BillingBatchEmailTypeID=@BillingBatchEmailTypeID, BillingID=@BillingID, EmailID=@EmailID WHERE BillingBatchEmailID=@BillingBatchEmailID;";

        public override void Save(BillingBatchEmail entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingBatchEmailID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingBatchEmailID", MySqlDbType.Int64).Value = entity.BillingBatchEmailID;
            }

            cmd.Parameters.Add("@BillingBatchEmailTypeID", MySqlDbType.Int32).Value = entity.BillingBatchEmailTypeID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@EmailID", MySqlDbType.Int64).Value = entity.EmailID;

            if (entity.BillingBatchEmailID == null)
            {
                entity.BillingBatchEmailID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingBatchEmail({0}) was not found in database.", entity.BillingBatchEmailID));
                }
            }
        }

        public override BillingBatchEmail Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override BillingBatchEmail Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
