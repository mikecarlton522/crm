using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class EmailDataProvider : EntityDataProvider<Email>
    {
        private const string INSERT_COMMAND = "INSERT INTO Email(DynamicEmailID, Email, Subject, Body, Response, CreateDT, BillingID) VALUES(@DynamicEmailID, @Email, @Subject, @Body, @Response, @CreateDT, @BillingID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Email SET DynamicEmailID=@DynamicEmailID, Email=@Email, Subject=@Subject, Body=@Body, Response=@Response, CreateDT=@CreateDT, BillingID=@BillingID WHERE EmailID=@EmailID;";
        //private const string INSERT_COMMAND = "INSERT INTO Email(DynamicEmailID, Email, Subject, Body, Response, CreateDT) VALUES(@DynamicEmailID, @Email, @Subject, @Body, @Response, @CreateDT); SELECT @@IDENTITY;";
        //private const string UPDATE_COMMAND = "UPDATE Email SET DynamicEmailID=@DynamicEmailID, Email=@Email, Subject=@Subject, Body=@Body, Response=@Response, CreateDT=@CreateDT WHERE EmailID=@EmailID;";
        private const string SELECT_COMMAND = "SELECT * FROM Email WHERE EmailID=@EmailID;";

        public override void Save(Email entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.EmailID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@EmailID", MySqlDbType.Int64).Value = entity.EmailID;
            }

            cmd.Parameters.Add("@DynamicEmailID", MySqlDbType.Int32).Value = entity.DynamicEmailID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = entity.BillingID;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = entity.Email_;
            cmd.Parameters.Add("@Subject", MySqlDbType.VarChar).Value = entity.Subject;

            if (!string.IsNullOrEmpty(entity.Body))
                cmd.Parameters.Add("@Body", MySqlDbType.Text).Value = "ZIP+BASE64" + StringCompressor.CompressString(entity.Body);
            else
                cmd.Parameters.Add("@Body", MySqlDbType.Text).Value =entity.Body;

            cmd.Parameters.Add("@Response", MySqlDbType.Text).Value = entity.Response;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.EmailID == null)
            {
                entity.EmailID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Email({0}) was not found in database.", entity.EmailID));
                }
            }
        }

        public override Email Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@EmailID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Email Load(DataRow row)
        {
            Email res = new Email();

            if (!(row["EmailID"] is DBNull))
                res.EmailID = Convert.ToInt32(row["EmailID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["Email"] is DBNull))
                res.Email_ = Convert.ToString(row["Email"]);
            if (!(row["Subject"] is DBNull))
                res.Subject = Convert.ToString(row["Subject"]);
            if (!(row["DynamicEmailID"] is DBNull))
                res.DynamicEmailID = Convert.ToInt32(row["DynamicEmailID"]);
            if (!(row["Body"] is DBNull))
                res.Body = Convert.ToString(row["Body"]);
            if (!string.IsNullOrEmpty(res.Body) && res.Body.StartsWith("ZIP+BASE64"))
            {
                if (res.Body.Length > 10)
                    res.Body = StringCompressor.DecompressString(res.Body.Substring(10, res.Body.Length - 10));
                else
                    res.Body = string.Empty;
            }
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
