using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TagGroupTagLinkDataProvider : EntityDataProvider<TagGroupTagLink>
    {
        private const string INSERT_COMMAND = "INSERT INTO TagGroupTagLink(TagGroupID, TagID) VALUES(@TagGroupID, @TagID);";

        public override void Save(TagGroupTagLink entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TagGroupTagLinkID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                throw new NotImplementedException();
            }

            cmd.Parameters.Add("@TagGroupID", MySqlDbType.Int32).Value = entity.TagGroupID;
            cmd.Parameters.Add("@TagID", MySqlDbType.Int32).Value = entity.TagID;

            if (entity.TagGroupTagLinkID == null)
            {
                cmd.ExecuteNonQuery();
                entity.TagGroupTagLinkID = new TagGroupTagLink.ID() { TagGroupID = entity.TagGroupID.Value, TagID = entity.TagID.Value };
            }
            else
            {
                throw new NotImplementedException();
            }            
        }

        public override TagGroupTagLink Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override TagGroupTagLink Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
