using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TPClientNoteDataProvider : EntityDataProvider<TPClientNote>
    {
        private const string SELECT_COMMAND = "SELECT * FROM TPClientNote WHERE TPClientNoteID=@TPClientNoteID;";
        private const string UPDATE_COMMAND = @"UPDATE TPClientNote SET TPClientID=@TPClientID, AdminID=@AdminID, Content=@Content, CreateDT=@CreateDT WHERE TPClientNoteID=@TPClientNoteID;";
        private const string INSERT_COMMAND = @"INSERT INTO TPClientNote(TPClientID, AdminID, Content, CreateDT) VALUES(@TPClientID, @AdminID, @Content, @CreateDT); SELECT @@IDENTITY;";

        public override void Save(TPClientNote entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TPClientNoteID == 0)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TPClientNoteID", MySqlDbType.Int32).Value = entity.TPClientNoteID;
            }

            cmd.Parameters.Add("@TPClientID", MySqlDbType.Int32).Value = entity.TPClientID;
            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = entity.AdminID;
            cmd.Parameters.Add("@Content", MySqlDbType.VarChar).Value = entity.Content;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Datetime).Value = entity.CreateDT;

            if (entity.TPClientNoteID == 0)
            {
                entity.TPClientNoteID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("TPClientNoteID({0}) was not found in database.", entity.TPClientNoteID));
                }
            }
        }

        public override TPClientNote Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TPClientNoteID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override TPClientNote Load(System.Data.DataRow row)
        {
            TPClientNote res = new TPClientNote();

            if (!(row["TPClientNoteID"] is DBNull))
                res.TPClientNoteID = Convert.ToInt32(row["TPClientNoteID"]);
            if (!(row["TPClientID"] is DBNull))
                res.TPClientID = Convert.ToInt32(row["TPClientID"]);
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
