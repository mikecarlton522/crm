using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class MIDCategoryDataProvider : EntityDataProvider<MIDCategory>
    {
        private const string SELECT_COMMAND = "SELECT * FROM MIDCategory WHERE MIDCategoryID = @MIDCategoryID;";
        private const string INSERT_COMMAND = "INSERT INTO MIDCategory (DisplayName) VALUES(@DisplayName);";
        private const string UPDATE_COMMAND = "UPDATE MIDCategory SET DisplayName=@DisplayName WHERE MIDCategoryID = @MIDCategoryID;";

        public override void Save(MIDCategory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            int? id = null;
            if (entity.MIDCategoryID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@MIDCategoryID", MySqlDbType.Int32).Value = entity.MIDCategoryID;
            }

            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;

            if (entity.MIDCategoryID == null)
            {
                entity.MIDCategoryID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("MIDCategory({0}) was not found in database.", entity.MIDCategoryID));
                }
            }
        }

        public override MIDCategory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@MIDCategoryID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override MIDCategory Load(DataRow row)
        {
            MIDCategory res = new MIDCategory();

            if (!(row["MIDCategoryID"] is DBNull))
                res.MIDCategoryID = Convert.ToInt32(row["MIDCategoryID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);

            return res;
        }
    }
}
