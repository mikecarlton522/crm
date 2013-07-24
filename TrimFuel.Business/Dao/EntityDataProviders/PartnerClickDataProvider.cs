using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PartnerClickDataProvider : EntityDataProvider<PartnerClick>
    {
        private const string INSERT_COMMAND = "INSERT INTO PartnerClick(BillingID, ClickID) VALUES(@BillingID, @ClickID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE PartnerClick SET BillingID=@BillingID, ClickID=@ClickID WHERE PartnerClickID=@PartnerClickID;";

        public override void Save(PartnerClick entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.PartnerClickID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@PartnerClickID", MySqlDbType.Int32).Value = entity.PartnerClickID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@ClickID", MySqlDbType.String).Value = entity.ClickID;

            if (entity.PartnerClickID == null)
            {
                entity.PartnerClickID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("PartnerClick({0}) was not found in database.", entity.PartnerClickID));
                }
            }
        }

        public override PartnerClick Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override PartnerClick Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
