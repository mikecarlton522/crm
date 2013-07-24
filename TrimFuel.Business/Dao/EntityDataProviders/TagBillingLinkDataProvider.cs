using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TagBillingLinkDataProvider : EntityDataProvider<TagBillingLink>
    {
        private const string INSERT_COMMAND = "INSERT INTO TagBillingLink(TagID, BillingID) VALUES(@TagID, @BillingID);";

        public override void Save(TagBillingLink entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TagBillingLinkID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                throw new NotImplementedException();
            }

            cmd.Parameters.Add("@TagID", MySqlDbType.Int32).Value = entity.TagID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;

            if (entity.TagBillingLinkID == null)
            {
                cmd.ExecuteNonQuery();
                entity.TagBillingLinkID = new TagBillingLink.ID() { TagID = entity.TagID.Value, BillingID = entity.BillingID.Value };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override TagBillingLink Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override TagBillingLink Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
