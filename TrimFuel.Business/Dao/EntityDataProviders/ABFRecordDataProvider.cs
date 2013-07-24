using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ABFRecordDataProvider : EntityDataProvider<ABFRecord>
    {
        private const string INSERT_COMMAND = "INSERT INTO ABFRecord(SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT) VALUES(@SaleID, @RegID, @Request, @Response, @StatusResponse, @Completed, @CreateDT, @ShippedDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ABFRecord SET SaleID=@SaleID, RegID=@RegID, Request=@Request, Response=@Response, StatusResponse=@StatusResponse, Completed=@Completed, CreateDT=@CreateDT, ShippedDT=@ShippedDT WHERE ABFRecordID=@ABFRecordID;";
        private const string SELECT_COMMAND = "SELECT * FROM ABFRecord WHERE ABFRecordID=@ABFRecordID;";

        public override void Save(ABFRecord entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ABFRecordID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ABFRecordID", MySqlDbType.Int32).Value = entity.ABFRecordID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RegID", MySqlDbType.Int64).Value = entity.RegID;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;
            cmd.Parameters.Add("@StatusResponse", MySqlDbType.VarChar).Value = entity.StatusResponse;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@ShippedDT", MySqlDbType.Timestamp).Value = entity.ShippedDT;

            if (entity.ABFRecordID == null)
            {
                entity.ABFRecordID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ABFRecord({0}) was not found in database.", entity.ABFRecordID));
                }
            }
        }

        public override ABFRecord Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ABFRecordID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ABFRecord Load(DataRow row)
        {
            ABFRecord res = new ABFRecord();

            if (!(row["ABFRecordID"] is DBNull))
                res.ABFRecordID = Convert.ToInt32(row["ABFRecordID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["RegID"] is DBNull))
                res.RegID = Convert.ToInt64(row["RegID"]);
            if (!(row["Request"] is DBNull))
                res.Request = Convert.ToString(row["Request"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["StatusResponse"] is DBNull))
                res.StatusResponse = Convert.ToString(row["StatusResponse"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["ShippedDT"] is DBNull))
                res.ShippedDT = Convert.ToDateTime(row["ShippedDT"]);

            return res;
        }
    }
}
