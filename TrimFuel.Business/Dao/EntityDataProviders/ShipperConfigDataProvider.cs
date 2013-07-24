using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipperConfigDataProvider : EntityDataProvider<ShipperConfig>
    {
        string SELECT_COMMAND = "SELECT * FROM ShipperConfig WHERE ShipperID=@ShipperID AND `Key`=@Key;";
        private const string INSERT_COMMAND = "INSERT INTO ShipperConfig(ShipperID, `Key`, `Value`) VALUES(@ShipperID, @Key, @Value);";
        private const string UPDATE_COMMAND = "UPDATE ShipperConfig SET ShipperID=@ShipperID, `Key`=@Key, `Value`=@Value WHERE ShipperID=@ID_ShipperID AND `Key`=@ID_Key;";

        public override void Save(ShipperConfig entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ShipperConfigID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ID_ShipperID", MySqlDbType.Int32).Value = entity.ShipperConfigID.Value.ShipperID;
                cmd.Parameters.Add("@ID_Key", MySqlDbType.VarChar).Value = entity.ShipperConfigID.Value.Key;
            }

            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = entity.ShipperID;
            cmd.Parameters.Add("@Key", MySqlDbType.VarChar).Value = entity.Key;
            cmd.Parameters.Add("@Value", MySqlDbType.VarChar).Value = entity.Value;


            if (entity.ShipperConfigID == null)
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ShipperConfig ({0}) was not found in database.", entity.Key));
                }
            }
            entity.ShipperConfigID = new ShipperConfig.ID() { Key = entity.Key, ShipperID = entity.ShipperID.Value };
        }

        public override ShipperConfig Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = ((ShipperConfig.ID)key).ShipperID;
            cmd.Parameters.Add("@Key", MySqlDbType.VarChar).Value = ((ShipperConfig.ID)key).Key;

            return Load(cmd).FirstOrDefault();
        }

        public override ShipperConfig Load(System.Data.DataRow row)
        {
            ShipperConfig res = new ShipperConfig();

            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["Key"] is DBNull))
                res.Key = Convert.ToString(row["Key"]);
            if (!(row["Value"] is DBNull))
                res.Value = Convert.ToString(row["Value"]);

            res.ShipperConfigID = new ShipperConfig.ID() { ShipperID = res.ShipperID.Value, Key = res.Key };

            return res;
        }
    }
}
