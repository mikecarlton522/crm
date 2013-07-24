using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipmentShippingNoteDataProvider : EntityDataProvider<ShipmentShippingNote>
    {
        private const string INSERT_COMMAND = "INSERT INTO ShipmentShippingNote(ShippingNoteID, ShipmentID) VALUES(@ShippingNoteID, @ShipmentID);";
        private const string UPDATE_COMMAND = "UPDATE ShipmentShippingNote SET ShippingNoteID=@ShippingNoteID, ShipmentID=@ShipmentID WHERE ShippingNoteID=@IDShippingNoteID AND ShipmentID=@IDShipmentID;";
        private const string SELECT_COMMAND = "SELECT * FROM ShipmentShippingNote WHERE ShippingNoteID=@IDShippingNoteID AND ShipmentID=@IDShipmentID;";

        public override void Save(ShipmentShippingNote entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ShipmentShippingNoteID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDShippingNoteID", MySqlDbType.Int64).Value = entity.ShipmentShippingNoteID.Value.ShippingNoteID;
                cmd.Parameters.Add("@IDShipmentID", MySqlDbType.Int64).Value = entity.ShipmentShippingNoteID.Value.ShipmentID;
            }

            cmd.Parameters.Add("@ShippingNoteID", MySqlDbType.Int64).Value = entity.ShippingNoteID;
            cmd.Parameters.Add("@ShipmentID", MySqlDbType.Int64).Value = entity.ShipmentID;


            if (entity.ShipmentShippingNoteID == null)
            {
                cmd.ExecuteNonQuery();
                entity.ShipmentShippingNoteID = new ShipmentShippingNote.ID() { ShippingNoteID = entity.ShippingNoteID.Value, ShipmentID = entity.ShipmentID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ShipmentShippingNote({0},{1}) was not found in database.", entity.ShipmentShippingNoteID.Value.ShippingNoteID, entity.ShipmentShippingNoteID.Value.ShipmentID));
                }
                else
                {
                    entity.ShipmentShippingNoteID = new ShipmentShippingNote.ID() { ShippingNoteID = entity.ShippingNoteID.Value, ShipmentID = entity.ShipmentID.Value };
                }
            }
        }

        public override ShipmentShippingNote Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDShippingNoteID", MySqlDbType.Int64).Value = ((ShipmentShippingNote.ID?)key).Value.ShippingNoteID;
            cmd.Parameters.Add("@IDShipmentID", MySqlDbType.Int64).Value = ((ShipmentShippingNote.ID?)key).Value.ShipmentID;

            return Load(cmd).FirstOrDefault();
        }

        public override ShipmentShippingNote Load(DataRow row)
        {
            ShipmentShippingNote res = new ShipmentShippingNote();

            if (!(row["ShippingNoteID"] is DBNull || row["ShipperID"] is DBNull))
                res.ShipmentShippingNoteID = new ShipmentShippingNote.ID()
                {
                    ShippingNoteID = Convert.ToInt64(row["ShippingNoteID"]),
                    ShipmentID = Convert.ToInt64(row["ShipmentID"])
                };
            if (!(row["ShippingNoteID"] is DBNull))
                res.ShippingNoteID = Convert.ToInt64(row["ShippingNoteID"]);
            if (!(row["ShipmentID"] is DBNull))
                res.ShipmentID = Convert.ToInt64(row["ShipmentID"]);

            return res;
        }
    }
}
