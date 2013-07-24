using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class WebStoreEmailsDataProvider : EntityDataProvider<WebStoreEmails>
    {
        private const string INSERT_COMMAND = "INSERT INTO WebStoreEmails (WebStoreID, Email) VALUES(@WebStoreID, @Email); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE WebStoreEmails SET WebStoreID=@WebStoreID, Email=@Email WHERE WebStoreEmailsID=@WebStoreEmailsID;";
        private const string SELECT_COMMAND = "SELECT * FROM WebStoreEmails WHERE WebStoreEmailsID=@WebStoreEmailsID;";

        public override void Save(WebStoreEmails entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.WebStoreEmailsID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@WebStoreEmailsID", MySqlDbType.Int32).Value = entity.WebStoreEmailsID;
            }

            cmd.Parameters.Add("@WebStoreID", MySqlDbType.Int32).Value = entity.WebStoreID;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = entity.Email;

            if (entity.WebStoreEmailsID == null)
            {
                entity.WebStoreEmailsID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("WebStoreEmails ({0}) was not found in database.", entity.WebStoreEmailsID));
                }
            }
        }

        public override WebStoreEmails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@WebStoreEmailsID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override WebStoreEmails Load(System.Data.DataRow row)
        {
            WebStoreEmails res = new WebStoreEmails();

            if (!(row["WebStoreEmailsID"] is DBNull))
                res.WebStoreEmailsID = Convert.ToInt32(row["WebStoreEmailsID"]);
            if (!(row["WebStoreID"] is DBNull))
                res.WebStoreID = Convert.ToInt32(row["WebStoreID"]);
            if (!(row["Email"] is DBNull))
                res.Email = Convert.ToString(row["Email"]);

            return res;
        }
    }
}
