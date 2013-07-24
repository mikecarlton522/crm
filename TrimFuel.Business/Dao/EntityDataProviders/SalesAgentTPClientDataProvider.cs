using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SalesAgentTPClientDataProvider : EntityDataProvider<SalesAgentTPClient>
    {
        private const string SELECT_COMMAND = "SELECT * FROM SalesAgentTPClient WHERE SalesAgentTPClientID=@SalesAgentTPClientID;";
        private const string INSERT_COMMAND = "INSERT INTO SalesAgentTPClient(SalesAgentID, TPClientID) VALUES(@SalesAgentID, @TPClientID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SalesAgentTPClient SET SalesAgentID=@SalesAgentID, TPClientID=@TPClientID WHERE SalesAgentTPClientID=@SalesAgentTPClientID;";

        public override void Save(SalesAgentTPClient entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SalesAgentTPClientID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SalesAgentTPClientID", MySqlDbType.Int32).Value = entity.SalesAgentTPClientID;
            }

            cmd.Parameters.Add("@SalesAgentID", MySqlDbType.Int64).Value = entity.SalesAgentID;
            cmd.Parameters.Add("@TPClientId", MySqlDbType.VarChar).Value = entity.TPClientID;
            

            if (entity.SalesAgentTPClientID == null)
            {
                entity.SalesAgentTPClientID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SalesAgentTPClient({0}) was not found in database.", entity.SalesAgentTPClientID));
                }
            }
        }

        public override SalesAgentTPClient Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SalesAgentTPClientID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SalesAgentTPClient Load(DataRow row)
        {
            SalesAgentTPClient res = new SalesAgentTPClient();

            if (!(row["SalesAgentTPClientID"] is DBNull))
                res.SalesAgentTPClientID = Convert.ToInt32(row["SalesAgentTPClientID"]);
            if (!(row["SalesAgentID"] is DBNull))
                res.SalesAgentID = Convert.ToInt32(row["SalesAgentID"]);
            if (!(row["TClientID"] is DBNull))
                res.TPClientID = Convert.ToInt32(row["TPClientID"]);

            return res;
        }
    }
}
