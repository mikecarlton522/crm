using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TPClientEmailDataProvider : EntityDataProvider<TPClientEmail>
    {
        private const string SELECT_COMMAND = "SELECT * FROM TPClientEmail WHERE TPClientEmailID=@TPClientEmailID;";
        private const string UPDATE_COMMAND = @"UPDATE TPClientEmail SET TPClientID=@TPClientID, AdminID=@AdminID, Content=@Content, CreateDT=@CreateDT, `From`=@From, `To`=@To, `Subject`=@Subject, Response=@Response WHERE TPClientEmailID=@TPClientEmailID;";
        private const string INSERT_COMMAND = @"INSERT INTO TPClientEmail(TPClientID, AdminID, Content, CreateDT, Response, `From`, `To`, `Subject`) VALUES(@TPClientID, @AdminID, @Content, @CreateDT, @Response, @From, @To, @Subject); SELECT @@IDENTITY;";

        public override void Save(TPClientEmail entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TPClientEmailID == 0)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TPClientEmailID", MySqlDbType.Int32).Value = entity.TPClientEmailID;
            }

            cmd.Parameters.Add("@TPClientID", MySqlDbType.Int32).Value = entity.TPClientID;
            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = entity.AdminID;
            cmd.Parameters.Add("@Content", MySqlDbType.VarChar).Value = entity.Content;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Datetime).Value = entity.CreateDT;
            cmd.Parameters.Add("@From", MySqlDbType.VarChar).Value = entity.From;
            cmd.Parameters.Add("@To", MySqlDbType.VarChar).Value = entity.To;
            cmd.Parameters.Add("@Subject", MySqlDbType.VarChar).Value = entity.Subject;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;

            if (entity.TPClientEmailID == 0)
            {
                entity.TPClientEmailID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("TPClientEmailID({0}) was not found in database.", entity.TPClientEmailID));
                }
            }
        }

        public override TPClientEmail Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TPClientEmailID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override TPClientEmail Load(System.Data.DataRow row)
        {
            TPClientEmail res = new TPClientEmail();

            if (!(row["TPClientEmailID"] is DBNull))
                res.TPClientEmailID = Convert.ToInt32(row["TPClientEmailID"]);
            if (!(row["TPClientID"] is DBNull))
                res.TPClientID = Convert.ToInt32(row["TPClientID"]);
            if (!(row["AdminID"] is DBNull))
                res.AdminID = Convert.ToInt32(row["AdminID"]);
            if (!(row["Content"] is DBNull))
                res.Content = Convert.ToString(row["Content"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["From"] is DBNull))
                res.From = Convert.ToString(row["From"]);
            if (!(row["To"] is DBNull))
                res.To = Convert.ToString(row["To"]);
            if (!(row["Subject"] is DBNull))
                res.Subject = Convert.ToString(row["Subject"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);

            return res;
        }
    }
}
