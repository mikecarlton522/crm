using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class IPRestrictionDataProvider : EntityDataProvider<IPRestriction>
    {
        private const string INSERT_COMMAND = "INSERT INTO IPRestriction(IP, RestrictLevelID) VALUES(@IP, @RestrictLevelID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE IPRestriction SET IP=@IP, RestrictLevelID=@RestrictLevelID WHERE IPRestrictionID=@IPRestrictionID;";
        private const string SELECT_COMMAND = "SELECT * FROM IPRestriction WHERE IPRestrictionID=@IPRestrictionID;";

        public override void Save(IPRestriction entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.IPRestrictionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IPRestrictionID", MySqlDbType.Int32).Value = entity.IPRestrictionID;
            }

            cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = entity.IP;
            cmd.Parameters.Add("@RestrictLevelID", MySqlDbType.Int32).Value = entity.RestrictLevelID;


            if (entity.IPRestrictionID == null)
            {
                entity.IPRestrictionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("IPRestriction({0}) was not found in database.", entity.IPRestrictionID));
                }
            }
        }

        public override IPRestriction Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IPRestrictionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override IPRestriction Load(DataRow row)
        {
            IPRestriction res = new IPRestriction();

            if (!(row["IPRestrictionID"] is DBNull))
                res.IPRestrictionID = Convert.ToInt32(row["IPRestrictionID"]);
            if (!(row["IP"] is DBNull))
                res.IP = Convert.ToString(row["IP"]);
            if (!(row["RestrictLevelID"] is DBNull))
                res.RestrictLevelID = Convert.ToInt32(row["RestrictLevelID"]);

            return res;
        }
    }
}
