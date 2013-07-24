using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SMTPSettingDataProvider : EntityDataProvider<SMTPSetting>
    {
        private const string SELECT_COMMAND = "SELECT * FROM SMTPSetting WHERE SMTPSettingID=@SMTPSettingID;";
        private const string INSERT_COMMAND = "INSERT INTO SMTPSetting(UserName, Password, Server, Port, EnableSSL, URL) VALUES(@UserName, @Password, @Server, @Port, @EnableSSL, @URL); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SMTPSetting SET UserName=@UserName, Password=@Password, Server=@Server, Port=@Port, EnableSSL=@EnableSSL, URL=@URL WHERE SMTPSettingID=@SMTPSettingID;";

        public override void Save(SMTPSetting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SMTPSettingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SMTPSettingID", MySqlDbType.Int32).Value = entity.SMTPSettingID;
            }

            cmd.Parameters.Add("@Port", MySqlDbType.Int32).Value = entity.Port;
            cmd.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = entity.UserName;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;
            cmd.Parameters.Add("@Server", MySqlDbType.VarChar).Value = entity.Server;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URL;
            cmd.Parameters.Add("@EnableSSL", MySqlDbType.Bit).Value = entity.EnableSSL;

            if (entity.SMTPSettingID == null)
            {
                entity.SMTPSettingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SMTPSetting ({0}) was not found in database.", entity.SMTPSettingID));
                }
            }
        }

        public override SMTPSetting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SMTPSettingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SMTPSetting Load(System.Data.DataRow row)
        {
            SMTPSetting res = new SMTPSetting();

            if (!(row["SMTPSettingID"] is DBNull))
                res.SMTPSettingID = Convert.ToInt32(row["SMTPSettingID"]);
            if (!(row["UserName"] is DBNull))
                res.UserName = Convert.ToString(row["UserName"]);
            if (!(row["Password"] is DBNull))
                res.Password = Convert.ToString(row["Password"]);
            if (!(row["Server"] is DBNull))
                res.Server = Convert.ToString(row["Server"]);
            if (!(row["URL"] is DBNull))
                res.URL = Convert.ToString(row["URL"]);
            if (!(row["Port"] is DBNull))
                res.Port = Convert.ToInt32(row["Port"]);
            if (!(row["EnableSSL"] is DBNull))
                res.EnableSSL = Convert.ToBoolean(row["EnableSSL"]);


            return res;
        }
    }
}
