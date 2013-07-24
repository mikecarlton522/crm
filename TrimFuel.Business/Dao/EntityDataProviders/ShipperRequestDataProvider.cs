using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipperRequestDataProvider : EntityDataProvider<ShipperRequest>
    {
        private const string INSERT_COMMAND = "INSERT INTO ShipperRequest(ShipperID, Request, Response, ResponseShipmentStatus, CreateDT) VALUES(@ShipperID, @Request, @Response, @ResponseShipmentStatus, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ShipperRequest SET ShipperID=@ShipperID, Request=@Request, Response=@Response, ResponseShipmentStatus=@ResponseShipmentStatus, CreateDT=@CreateDT WHERE ShipperRequestID=@ShipperRequestID;";
        private const string SELECT_COMMAND = "SELECT * FROM ShipperRequest WHERE ShipperRequestID=@ShipperRequestID;";

        public override void Save(ShipperRequest entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ShipperRequestID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ShipperRequestID", MySqlDbType.Int64).Value = entity.ShipperRequestID;
            }

            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int16).Value = entity.ShipperID;
            cmd.Parameters.Add("@Request", MySqlDbType.Text).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.Text).Value = entity.Response;
            cmd.Parameters.Add("@ResponseShipmentStatus", MySqlDbType.Int64).Value = entity.ResponseShipmentStatus;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.ShipperRequestID == null)
            {
                entity.ShipperRequestID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ShipperRequest({0}) was not found in database.", entity.ShipperRequestID));
                }
            }
        }

        public override ShipperRequest Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ShipperRequestID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ShipperRequest Load(DataRow row)
        {
            ShipperRequest res = new ShipperRequest();

            if (!(row["ShipperRequestID"] is DBNull))
                res.ShipperRequestID = Convert.ToInt64(row["ShipperRequestID"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt16(row["ShipperID"]);
            if (!(row["Request"] is DBNull))
                res.Request = Convert.ToString(row["Request"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["ResponseShipmentStatus"] is DBNull))
                res.ResponseShipmentStatus = Convert.ToInt32(row["ResponseShipmentStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
