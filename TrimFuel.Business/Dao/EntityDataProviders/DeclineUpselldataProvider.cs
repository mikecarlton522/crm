using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class DeclineUpsellDataProvider : EntityDataProvider<DeclineUpsell>
    {
        private const string INSERT_COMMAND = "INSERT INTO DeclineUpsell(BillingID, ChargeHistoryID) VALUES(@BillingID, @ChargeHistoryID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE DeclineUpsell SET BillingID=@BillingID, ChargeHistoryID=@ChargeHistoryID WHERE ID=@DeclineUpsellID;";

        public override void Save(DeclineUpsell entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.DeclineUpsellID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@DeclineUpsellID", MySqlDbType.Int32).Value = entity.DeclineUpsellID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;

            if (entity.DeclineUpsellID == null)
            {
                entity.DeclineUpsellID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("DeclineUpsell({0}) was not found in database.", entity.DeclineUpsellID));
                }
            }
        }

        public override DeclineUpsell Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override DeclineUpsell Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
