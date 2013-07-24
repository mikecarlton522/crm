using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RegistrationInfoDataProvider : EntityDataProvider<RegistrationInfo>
    {
        private const string INSERT_COMMAND = "INSERT INTO RegistrationInfo(RegistrationID, Country, Neighborhood, CustomField1, CustomField2, CustomField3, CustomField4) VALUES(@RegistrationID, @Country, @Neighborhood, @CustomField1, @CustomField2, @CustomField3, @CustomField4); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RegistrationInfo SET RegistrationID=@RegistrationID, Country=@Country, Neighborhood=@Neighborhood, CustomField1=@CustomField1, CustomField2=@CustomField2, CustomField3=@CustomField3, CustomField4=@CustomField4 WHERE RegistrationInfoID=@RegistrationInfoID;";
        private const string SELECT_COMMAND = "SELECT * FROM RegistrationInfo WHERE RegistrationInfoID=@RegistrationInfoID;";

        public override void Save(RegistrationInfo entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RegistrationInfoID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RegistrationInfoID", MySqlDbType.Int64).Value = entity.RegistrationInfoID;
            }

            cmd.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = entity.RegistrationID;
            cmd.Parameters.Add("@Country", MySqlDbType.VarChar).Value = entity.Country;
            cmd.Parameters.Add("@Neighborhood", MySqlDbType.VarChar).Value = entity.Neighborhood;
            cmd.Parameters.Add("@CustomField1", MySqlDbType.VarChar).Value = entity.CustomField1;
            cmd.Parameters.Add("@CustomField2", MySqlDbType.VarChar).Value = entity.CustomField2;
            cmd.Parameters.Add("@CustomField3", MySqlDbType.VarChar).Value = entity.CustomField3;
            cmd.Parameters.Add("@CustomField4", MySqlDbType.VarChar).Value = entity.CustomField4;

            if (entity.RegistrationInfoID == null)
            {
                entity.RegistrationInfoID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RegistrationInfo({0}) was not found in database.", entity.RegistrationInfoID));
                }
            }
        }

        public override RegistrationInfo Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RegistrationInfoID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RegistrationInfo Load(DataRow row)
        {
            RegistrationInfo res = new RegistrationInfo();

            if (!(row["RegistrationInfoID"] is DBNull))
                res.RegistrationInfoID = Convert.ToInt64(row["RegistrationInfoID"]);
            if (!(row["RegistrationID"] is DBNull))
                res.RegistrationID = Convert.ToInt64(row["RegistrationID"]);
            if (!(row["Country"] is DBNull))
                res.Country = Convert.ToString(row["Country"]);
            if (!(row["Neighborhood"] is DBNull))
                res.Neighborhood = Convert.ToString(row["Neighborhood"]);
            if (!(row["CustomField1"] is DBNull))
                res.CustomField1 = Convert.ToString(row["CustomField1"]);
            if (!(row["CustomField2"] is DBNull))
                res.CustomField2 = Convert.ToString(row["CustomField2"]);
            if (!(row["CustomField3"] is DBNull))
                res.CustomField3 = Convert.ToString(row["CustomField3"]);
            if (!(row["CustomField4"] is DBNull))
                res.CustomField4 = Convert.ToString(row["CustomField4"]);

            return res;
        }
    }
}
