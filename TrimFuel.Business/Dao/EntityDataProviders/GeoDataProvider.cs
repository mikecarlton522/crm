using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class GeoDataProvider : EntityDataProvider<Geo>
    {
        private const string INSERT_COMMAND = "INSERT INTO Geo(GeoTypeID, Code, Name) VALUES(@GeoTypeID, @Code, @Name); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Geo SET GeoTypeID=@GeoTypeID, Code=@Code, Name=@Name WHERE GeoID=@GeoID;";
        private const string SELECT_COMMAND = "SELECT * FROM Geo WHERE GeoID=@GeoID;";

        public override void Save(Geo entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.GeoID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@GeoID", MySqlDbType.Int32).Value = entity.GeoID;
            }

            cmd.Parameters.Add("@GeoTypeID", MySqlDbType.Int32).Value = entity.GeoTypeID;
            cmd.Parameters.Add("@Code", MySqlDbType.VarChar).Value = entity.Code;
            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;


            if (entity.GeoID == null)
            {
                entity.GeoID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Geo({0}) was not found in database.", entity.GeoID));
                }
            }
        }

        public override Geo Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@GeoID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Geo Load(DataRow row)
        {
            Geo res = new Geo();

            if (!(row["GeoID"] is DBNull))
                res.GeoID = Convert.ToInt32(row["GeoID"]);
            if (!(row["GeoTypeID"] is DBNull))
                res.GeoTypeID = Convert.ToInt32(row["GeoTypeID"]);
            if (!(row["Code"] is DBNull))
                res.Code = Convert.ToString(row["Code"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);

            return res;
        }
    }
}
