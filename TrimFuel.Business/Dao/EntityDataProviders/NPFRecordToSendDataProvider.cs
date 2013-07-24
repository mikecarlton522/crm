using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class NPFRecordToSendDataProvider : EntityDataProvider<NPFRecordToSend>
    {
        private const string INSERT_COMMAND = "INSERT INTO NPFRecordToSend(SaleID, RegID, Request, CreateDT) VALUES(@SaleID, @RegID, @Request, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE NPFRecordToSend SET SaleID=@SaleID, RegID=@RegID, Request=@Request, CreateDT=@CreateDT WHERE NPFRecordToSendID=@NPFRecordToSendID;";
        private const string SELECT_COMMAND = "SELECT * FROM NPFRecordToSend WHERE NPFRecordToSendID=@NPFRecordToSendID;";

        public override void Save(NPFRecordToSend entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.NPFRecordToSendID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@NPFRecordToSendID", MySqlDbType.Int32).Value = entity.NPFRecordToSendID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RegID", MySqlDbType.Int64).Value = entity.RegID;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.NPFRecordToSendID == null)
            {
                entity.NPFRecordToSendID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("NPFRecordToSend({0}) was not found in database.", entity.NPFRecordToSendID));
                }
            }
        }

        public override NPFRecordToSend Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@NPFRecordToSendID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override NPFRecordToSend Load(DataRow row)
        {
            NPFRecordToSend res = new NPFRecordToSend();

            if (!(row["NPFRecordToSendID"] is DBNull))
                res.NPFRecordToSendID = Convert.ToInt32(row["NPFRecordToSendID"]);
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
