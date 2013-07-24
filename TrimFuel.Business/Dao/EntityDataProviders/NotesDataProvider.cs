using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class NotesDataProvider : EntityDataProvider<Notes>
    {
        private const string INSERT_COMMAND = "INSERT INTO Notes(BillingID, AdminID, Content, CreateDT) VALUES(@BillingID, @AdminID, @Content, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Notes SET BillingID=@BillingID, AdminID=@AdminID, Content=@Content, CreateDT=@CreateDT WHERE NotesID=@NotesID;";
        private const string SELECT_COMMAND = "SELECT * FROM Notes WHERE NotesID=@NotesID;";

        public override void Save(Notes entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.NotesID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@NotesID", MySqlDbType.Int32).Value = entity.NotesID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = entity.BillingID;
            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = entity.AdminID;
            cmd.Parameters.Add("@Content", MySqlDbType.Text).Value = entity.Content;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.NotesID == null)
            {
                entity.NotesID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Notes({0}) was not found in database.", entity.NotesID));
                }
            }
        }

        public override Notes Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@NotesID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Notes Load(DataRow row)
        {
            Notes res = new Notes();

            if (!(row["NotesID"] is DBNull))
                res.NotesID = Convert.ToInt32(row["NotesID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["AdminID"] is DBNull))
                res.AdminID = Convert.ToInt32(row["AdminID"]);
            if (!(row["Content"] is DBNull))
                res.Content = Convert.ToString(row["Content"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
