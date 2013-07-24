using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TPClientNewsDataProvider : EntityDataProvider<TPClientNews>
    {

        private const string INSERT_COMMAND = "INSERT INTO `TPClientNews`(AdminID, Content, CreateDT, Active) VALUES(@AdminID, @Content, @CreateDT, @Active);";
        private const string UPDATE_COMMAND = "UPDATE `TPClientNews` SET AdminID=@AdminID, Content=@Content, CreateDT=@CreateDT, Active=@Active WHERE TPClientNewsID=@TPClientNewsID;";
        private const string SELECT_COMMAND = "SELECT * FROM `TPClientNews` WHERE TPClientNewsID=@TPClientNewsID;";

        public override void Save(TPClientNews entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TPClientNewsID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TPClientNewsID", MySqlDbType.Int32).Value = entity.TPClientNewsID;
            }

            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = entity.AdminID;
            cmd.Parameters.Add("@Content", MySqlDbType.VarChar).Value = entity.Content;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Active", MySqlDbType.Bit).Value = entity.Active;

            if (entity.TPClientNewsID == null)
            {
                entity.TPClientNewsID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("TPClientNews({0}) was not found in database.", entity.TPClientNewsID));
                }
            }
        }
        public override TPClientNews Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TPClientNewsID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }
        public override TPClientNews Load(DataRow row)
        {
            TPClientNews res = new TPClientNews();

            if (!(row["TPClientNewsID"] is DBNull))
                res.TPClientNewsID = Convert.ToInt32(row["TPClientNewsID"]);
            if (!(row["AdminID"] is DBNull))
                res.AdminID = Convert.ToInt32(row["AdminID"]);
            if (!(row["Content"] is DBNull))
                res.Content = Convert.ToString(row["Content"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);

            return res;
        }

    }
}
