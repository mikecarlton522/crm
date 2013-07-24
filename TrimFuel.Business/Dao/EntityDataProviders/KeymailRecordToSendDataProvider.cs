using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class KeymailRecordToSendDataProvider : EntityDataProvider<KeymailRecordToSend>
    {
        private const string INSERT_COMMAND = "INSERT INTO KeymailRecordToSend(SaleID, RegID, Request, CreateDT) VALUES(@SaleID, @RegID, @Request, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE KeymailRecordToSend SET SaleID=@SaleID, RegID=@RegID, Request=@Request, CreateDT=@CreateDT WHERE KeymailRecordToSendID=@KeymailRecordToSendID;";
        private const string SELECT_COMMAND = "SELECT * FROM KeymailRecordToSend WHERE KeymailRecordToSendID=@KeymailRecordToSendID;";

        public override void Save(KeymailRecordToSend entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.KeymailRecordToSendID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@KeymailRecordToSendID", MySqlDbType.Int32).Value = entity.KeymailRecordToSendID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RegID", MySqlDbType.Int64).Value = entity.RegID;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.KeymailRecordToSendID == null)
            {
                entity.KeymailRecordToSendID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("KeymailRecordToSend({0}) was not found in database.", entity.KeymailRecordToSendID));
                }
            }
        }

        public override KeymailRecordToSend Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@KeymailRecordToSendID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override KeymailRecordToSend Load(DataRow row)
        {
            KeymailRecordToSend res = new KeymailRecordToSend();

            if (!(row["KeymailRecordToSendID"] is DBNull))
                res.KeymailRecordToSendID = Convert.ToInt32(row["KeymailRecordToSendID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["RegID"] is DBNull))
                res.RegID = Convert.ToInt64(row["RegID"]);
            if (!(row["Request"] is DBNull))
                res.Request = Convert.ToString(row["Request"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
