using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipmentShipperRequestDataProvider : EntityDataProvider<ShipmentShipperRequest>
    {
        private const string INSERT_COMMAND = "INSERT INTO ShipmentShipperRequest(ShipperRequestID, ShipmentID) VALUES(@ShipperRequestID, @ShipmentID);";
        private const string UPDATE_COMMAND = "UPDATE ShipmentShipperRequest SET ShipperRequestID=@ShipperRequestID, ShipmentID=@ShipmentID WHERE ShipperRequestID=@IDShipperRequestID AND ShipmentID=@IDShipmentID;";
        private const string SELECT_COMMAND = "SELECT * FROM ShipmentShipperRequest WHERE ShipperRequestID=@IDShipperRequestID AND ShipmentID=@IDShipmentID;";

        public override void Save(ShipmentShipperRequest entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ShipmentShipperRequestID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDShipperRequestID", MySqlDbType.Int64).Value = entity.ShipmentShipperRequestID.Value.ShipperRequestID;
                cmd.Parameters.Add("@IDShipmentID", MySqlDbType.Int64).Value = entity.ShipmentShipperRequestID.Value.ShipmentID;
            }

            cmd.Parameters.Add("@ShipperRequestID", MySqlDbType.Int64).Value = entity.ShipperRequestID;
            cmd.Parameters.Add("@ShipmentID", MySqlDbType.Int64).Value = entity.ShipmentID;


            if (entity.ShipmentShipperRequestID == null)
            {
                cmd.ExecuteNonQuery();
                entity.ShipmentShipperRequestID = new ShipmentShipperRequest.ID() { ShipperRequestID = entity.ShipperRequestID.Value, ShipmentID = entity.ShipmentID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ShipmentShipperRequest({0},{1}) was not found in database.", entity.ShipmentShipperRequestID.Value.ShipperRequestID, entity.ShipmentShipperRequestID.Value.ShipmentID));
                }
                else
                {
                    entity.ShipmentShipperRequestID = new ShipmentShipperRequest.ID() { ShipperRequestID = entity.ShipperRequestID.Value, ShipmentID = entity.ShipmentID.Value };
                }
            }
        }

        public override ShipmentShipperRequest Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDShipperRequestID", MySqlDbType.Int64).Value = ((ShipmentShipperRequest.ID?)key).Value.ShipperRequestID;
            cmd.Parameters.Add("@IDShipmentID", MySqlDbType.Int64).Value = ((ShipmentShipperRequest.ID?)key).Value.ShipmentID;

            return Load(cmd).FirstOrDefault();
        }

        public override ShipmentShipperRequest Load(DataRow row)
        {
            ShipmentShipperRequest res = new ShipmentShipperRequest();

            if (!(row["ShipperRequestID"] is DBNull || row["ShipperID"] is DBNull))
                res.ShipmentShipperRequestID = new ShipmentShipperRequest.ID()
                {
                    ShipperRequestID = Convert.ToInt64(row["ShipperRequestID"]),
                    ShipmentID = Convert.ToInt64(row["ShipmentID"])
                };
            if (!(row["ShipperRequestID"] is DBNull))
                res.ShipperRequestID = Convert.ToInt64(row["ShipperRequestID"]);
            if (!(row["ShipmentID"] is DBNull))
                res.ShipmentID = Convert.ToInt64(row["ShipmentID"]);

            return res;
        }
    }
}
