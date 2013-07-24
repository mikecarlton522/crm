using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SHWRegistrationDataProvider : EntityDataProvider<SHWRegistration>
    {
        private const string INSERT_COMMAND = "INSERT INTO SHWRegistration(BillingID, Request, Response, SHWID, CreateDT) VALUES(@BillingID, @Request, @Response, @SHWID, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SHWRegistration SET BillingID=@BillingID, Request=@Request, Response=@Response, SHWID=@SHWID, CreateDT=@CreateDT WHERE SHWRegistrationID=@SHWRegistrationID;";
        private const string SELECT_COMMAND = "SELECT * FROM SHWRegistration WHERE SHWRegistration=@SHWRegistrationID;";

        public override void Save(SHWRegistration entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SHWRegistrationID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SHWRegistrationID", MySqlDbType.Int32).Value = entity.SHWRegistrationID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@Request", MySqlDbType.Text).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.Text).Value = entity.Response;
            cmd.Parameters.Add("@SHWID", MySqlDbType.VarChar).Value = entity.SHWID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.SHWRegistrationID == null)
            {
                entity.SHWRegistrationID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SHWRegistration({0}) was not found in database.", entity.SHWRegistrationID));
                }
            }
        }

        public override SHWRegistration Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SHWRegistrationID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SHWRegistration Load(DataRow row)
        {
            SHWRegistration res = new SHWRegistration();

            if (!(row["SHWRegistrationID"] is DBNull))
                res.SHWRegistrationID = Convert.ToInt32(row["SHWRegistrationID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["Request"] is DBNull))
                res.Request = Convert.ToString(row["Request"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["SHWID"] is DBNull))
                res.SHWID = Convert.ToString(row["SHWID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
