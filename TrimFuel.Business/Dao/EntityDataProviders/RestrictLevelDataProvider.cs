using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RestrictLevelDataProvider : EntityDataProvider<RestrictLevel>
    {
        private const string INSERT_COMMAND = "INSERT INTO RestrictLevel(DisplayName, StartAdminPageID, AllowAllIP) VALUES(@DisplayName, @StartAdminPageID, @AllowAllIP); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RestrictLevel SET DisplayName=@DisplayName, StartAdminPageID=@StartAdminPageID, AllowAllIP=@AllowAllIP WHERE RestrictLevelID=@RestrictLevelID;";
        private const string SELECT_COMMAND = "SELECT * FROM RestrictLevel WHERE RestrictLevelID=@RestrictLevelID;";

        public override void Save(RestrictLevel entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RestrictLevelID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RestrictLevelID", MySqlDbType.Int32).Value = entity.RestrictLevelID;
            }

            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@StartAdminPageID", MySqlDbType.Int32).Value = entity.StartAdminPageID;
            cmd.Parameters.Add("@AllowAllIP", MySqlDbType.Bit).Value = entity.AllowAllIP;

            if (entity.RestrictLevelID == null)
            {
                entity.RestrictLevelID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RestrictLevel({0}) was not found in database.", entity.RestrictLevelID));
                }
            }
        }

        public override RestrictLevel Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RestrictLevelID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RestrictLevel Load(DataRow row)
        {
            RestrictLevel res = new RestrictLevel();

            if (!(row["RestrictLevelID"] is DBNull))
                res.RestrictLevelID = Convert.ToInt32(row["RestrictLevelID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["StartAdminPageID"] is DBNull))
                res.StartAdminPageID = Convert.ToInt32(row["StartAdminPageID"]);
            if (!(row["AllowAllIP"] is DBNull))
                res.AllowAllIP = Convert.ToBoolean(row["AllowAllIP"]);

            return res;
        }
    }
}
