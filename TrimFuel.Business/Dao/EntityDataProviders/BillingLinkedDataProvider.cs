using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingLinkedDataProvider : EntityDataProvider<BillingLinked>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingLinked(ParentBillingID, BillingID) VALUES(@ParentBillingID, @BillingID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingLinked SET ParentBillingID=@ParentBillingID, BillingID=@BillingID WHERE BillingLinkedID=@BillingLinkedID;";

        public override void Save(BillingLinked entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingLinkedID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingLinkedID", MySqlDbType.Int32).Value = entity.BillingLinkedID;
            }

            cmd.Parameters.Add("@ParentBillingID", MySqlDbType.Int64).Value = entity.ParentBillingID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;

            if (entity.BillingLinkedID == null)
            {
                entity.BillingLinkedID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingLinked({0}) was not found in database.", entity.BillingLinkedID));
                }
            }
        }

        public override BillingLinked Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override BillingLinked Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
