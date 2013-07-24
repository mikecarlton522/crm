using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingReferredDataProvider : EntityDataProvider<BillingReferred>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingReferred(BillingID, ReferralBillingID, CreateDT, ExtraGiftCompleted) VALUES(@BillingID, @ReferralBillingID, @CreateDT, @ExtraGiftCompleted); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingReferred SET BillingID=@BillingID, ReferralBillingID=@ReferralBillingID, CreateDT=@CreateDT, ExtraGiftCompleted=@ExtraGiftCompleted WHERE BillingReferredID=@BillingReferredID;";
        private const string SELECT_COMMAND = "SELECT * FROM BillingReferred WHERE BillingReferredID=@BillingReferredID;";

        public override void Save(BillingReferred entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingReferredID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingReferredID", MySqlDbType.Int32).Value = entity.BillingReferredID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = entity.BillingID;
            cmd.Parameters.Add("@ReferralBillingID", MySqlDbType.Int32).Value = entity.ReferralBillingID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@ExtraGiftCompleted", MySqlDbType.Bit).Value = entity.ExtraGiftCompleted;

            if (entity.BillingReferredID == null)
            {
                entity.BillingReferredID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingReferred({0}) was not found in database.", entity.BillingReferredID));
                }
            }
        }

        public override BillingReferred Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingReferredID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingReferred Load(DataRow row)
        {
            BillingReferred res = new BillingReferred();

            if (!(row["BillingReferredID"] is DBNull))
                res.BillingReferredID = Convert.ToInt32(row["BillingReferredID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["ReferralBillingID"] is DBNull))
                res.ReferralBillingID = Convert.ToInt32(row["ReferralBillingID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["ExtraGiftCompleted"] is DBNull))
                res.ExtraGiftCompleted = Convert.ToBoolean(row["ExtraGiftCompleted"]);

            return res;
        }
    }
}
