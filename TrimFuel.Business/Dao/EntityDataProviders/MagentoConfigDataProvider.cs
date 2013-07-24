using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class MagentoConfigDataProvider : EntityDataProvider<MagentoConfig>
    {
        private const string INSERT_COMMAND = "INSERT INTO `MagentoConfig`(MagentoURL, User, Password, Active ) VALUES(@MagentoURL, @User, @Password, @Active);";
        private const string UPDATE_COMMAND = "UPDATE `MagentoConfig` SET MagentoURL=@MagentoURL, User=@User, Password=@Password, Active=@Active;";
        private const string SELECT_COMMAND = "SELECT * FROM `MagentoConfig`;";

        public override void Save(MagentoConfig entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.MagentoURL == null)
            {
                cmd.CommandText = UPDATE_COMMAND;
            }
            else
            {
                cmd.CommandText = INSERT_COMMAND;
            }

            cmd.Parameters.Add("@MagentoURL", MySqlDbType.VarChar).Value = entity.MagentoURL;
            cmd.Parameters.Add("@User", MySqlDbType.VarChar).Value = entity.User;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;
            cmd.Parameters.Add("@Active", MySqlDbType.Bit).Value = entity.Active;

            if (entity.MagentoURL == null)
            {
                cmd.ExecuteScalar();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Magento URL ({0}) was not found in database.", entity.MagentoURL));
                }
            }
        }

        public override MagentoConfig Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            return Load(cmd).FirstOrDefault();
        }

        public override MagentoConfig Load(DataRow row)
        {
            MagentoConfig res = new MagentoConfig();

            if (!(row["MagentoURL"] is DBNull))
                res.MagentoURL = Convert.ToString(row["MagentoURL"]);
            if (!(row["User"] is DBNull))
                res.User = Convert.ToString(row["User"]);
            if (!(row["Password"] is DBNull))
                res.Password = Convert.ToString(row["Password"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);

            return res;
        }
    }
}
