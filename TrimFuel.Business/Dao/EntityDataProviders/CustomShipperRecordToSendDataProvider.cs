using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CustomShipperRecordToSendDataProvider : EntityDataProvider<CustomShipperRecordToSend>
    {
        private const string INSERT_COMMAND = "INSERT INTO CustomShipperRecordToSend(SaleID, RegID, Request, CreateDT) VALUES(@SaleID, @RegID, @Request, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE CustomShipperRecordToSend SET SaleID=@SaleID, RegID=@RegID, Request=@Request, CreateDT=@CreateDT WHERE CustomShipperRecordToSendID=@CustomShipperRecordToSendID;";
        private const string SELECT_COMMAND = "SELECT * FROM CustomShipperRecordToSend WHERE CustomShipperRecordToSendID=@CustomShipperRecordToSendID;";

        public override void Save(CustomShipperRecordToSend entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CustomShipperRecordToSendID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CustomShipperRecordToSendID", MySqlDbType.Int32).Value = entity.CustomShipperRecordToSendID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RegID", MySqlDbType.Int64).Value = entity.RegID;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.CustomShipperRecordToSendID == null)
            {
                entity.CustomShipperRecordToSendID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("CustomShipperRecordToSend({0}) was not found in database.", entity.CustomShipperRecordToSendID));
                }
            }
        }

        public override CustomShipperRecordToSend Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CustomShipperRecordToSendID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override CustomShipperRecordToSend Load(DataRow row)
        {
            CustomShipperRecordToSend res = new CustomShipperRecordToSend();

            if (!(row["CustomShipperRecordToSendID"] is DBNull))
                res.CustomShipperRecordToSendID = Convert.ToInt32(row["CustomShipperRecordToSendID"]);
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
