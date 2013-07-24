using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShippingNoteDataProvider : EntityDataProvider<ShippingNote>
    {
        private const string INSERT_COMMAND = "INSERT INTO ShippingNote(Note, NoteShipmentStatus, CreateDT) VALUES(@Note, @NoteShipmentStatus, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ShippingNote SET Note=@Note, NoteShipmentStatus=@NoteShipmentStatus, CreateDT=@CreateDT WHERE ShippingNoteID=@ShippingNoteID;";
        private const string SELECT_COMMAND = "SELECT * FROM ShippingNote WHERE ShippingNoteID=@ShippingNoteID;";

        public override void Save(ShippingNote entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ShippingNoteID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ShippingNoteID", MySqlDbType.Int64).Value = entity.ShippingNoteID;
            }

            cmd.Parameters.Add("@Note", MySqlDbType.VarChar).Value = entity.Note;
            cmd.Parameters.Add("@NoteShipmentStatus", MySqlDbType.Int32).Value = entity.NoteShipmentStatus;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (entity.ShippingNoteID == null)
            {
                entity.ShippingNoteID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ShippingNote({0}) was not found in database.", entity.ShippingNoteID));
                }
            }
        }

        public override ShippingNote Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ShippingNoteID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ShippingNote Load(DataRow row)
        {
            ShippingNote res = new ShippingNote();

            if (!(row["ShippingNoteID"] is DBNull))
                res.ShippingNoteID = Convert.ToInt64(row["ShippingNoteID"]);
            if (!(row["Note"] is DBNull))
                res.Note = Convert.ToString(row["Note"]);
            if (!(row["NoteShipmentStatus"] is DBNull))
                res.NoteShipmentStatus = Convert.ToInt32(row["NoteShipmentStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
