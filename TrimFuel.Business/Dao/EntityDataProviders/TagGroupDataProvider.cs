using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TagGroupDataProvider : EntityDataProvider<TagGroup>
    {
        private const string INSERT_COMMAND = "INSERT INTO TagGroup(TagGroupValue) VALUES(@TagGroupValue); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE TagGroup SET TagGroupValue=@TagGroupValue WHERE TagGroupID=@TagGroupID;";
        private const string SELECT_COMMAND = "SELECT * FROM TagGroup WHERE TagGroupID=@TagGroupID;";

        public override void Save(TagGroup entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TagGroupID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TagGroupID", MySqlDbType.Int32).Value = entity.TagGroupID;
            }

            cmd.Parameters.Add("@TagGroupValue", MySqlDbType.VarChar).Value = entity.TagGroupValue;

            if (entity.TagGroupID == null)
            {
                entity.TagGroupID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("TagGroup({0}) was not found in database.", entity.TagGroupID));
                }
            }
        }

        public override TagGroup Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TagGroupID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override TagGroup Load(DataRow row)
        {
            TagGroup res = new TagGroup();

            if (!(row["TagGroupID"] is DBNull))
                res.TagGroupID = Convert.ToInt32(row["TagGroupID"]);
            if (!(row["TagGroupValue"] is DBNull))
                res.TagGroupValue = Convert.ToString(row["TagGroupValue"]);

            return res;
        }
    }
}
