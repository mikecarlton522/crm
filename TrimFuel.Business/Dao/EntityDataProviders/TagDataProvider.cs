using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TagDataProvider : EntityDataProvider<Tag>
    {
        private const string INSERT_COMMAND = "INSERT INTO Tag(TagValue) VALUES(@TagValue); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Tag SET TagValue=@TagValue WHERE TagID=@TagID;";
        private const string SELECT_COMMAND = "SELECT * FROM Tag WHERE TagID=@TagID;";

        public override void Save(Tag entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TagID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TagID", MySqlDbType.Int32).Value = entity.TagID;
            }

            cmd.Parameters.Add("@TagValue", MySqlDbType.VarChar).Value = entity.TagValue;

            if (entity.TagID == null)
            {
                entity.TagID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Tag({0}) was not found in database.", entity.TagID));
                }
            }
        }

        public override Tag Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TagID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Tag Load(DataRow row)
        {
            Tag res = new Tag();

            if (!(row["TagID"] is DBNull))
                res.TagID = Convert.ToInt32(row["TagID"]);
            if (!(row["TagValue"] is DBNull))
                res.TagValue = Convert.ToString(row["TagValue"]);

            return res;
        }
    }
}
