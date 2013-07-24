using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class NMICompanyDataProvider : EntityDataProvider<NMICompany>
    {
        private const string INSERT_COMMAND = "INSERT INTO NMICompany(NMICompanyID, CompanyName, GatewayUsername, GatewayPassword, GatewayIntegrated, Active, Deleted) VALUES(@NMICompanyID, @CompanyName, @GatewayUsername, @GatewayPassword, @GatewayIntegrated, @Active, @Deleted);";
        private const string UPDATE_COMMAND = "UPDATE NMICompany SET NMICompanyID=@NMICompanyID, CompanyName=@CompanyName, GatewayUsername=@GatewayUsername, GatewayPassword=@GatewayPassword, GatewayIntegrated=@GatewayIntegrated, Active=@Active, Deleted=@Deleted WHERE NMICompanyID=@NMICompanyID;";
        private const string SELECT_COMMAND = "SELECT * FROM NMICompany WHERE NMICompanyID=@NMICompanyID;";

        public override void Save(NMICompany entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            int? id = null;
            if (entity.NMICompanyID == null)
            {
                id = GetNewID(cmdCreater);
                cmd.CommandText = INSERT_COMMAND;
                cmd.Parameters.Add("@NMICompanyID", MySqlDbType.Int32).Value = id;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@NMICompanyID", MySqlDbType.Int32).Value = entity.NMICompanyID;
            }

            cmd.Parameters.Add("@CompanyName", MySqlDbType.VarChar).Value = entity.CompanyName;
            cmd.Parameters.Add("@GatewayUsername", MySqlDbType.VarChar).Value = entity.GatewayUsername;
            cmd.Parameters.Add("@GatewayPassword", MySqlDbType.VarChar).Value = entity.GatewayPassword;
            cmd.Parameters.Add("@GatewayIntegrated", MySqlDbType.VarChar).Value = entity.GatewayIntegrated;
            cmd.Parameters.Add("@Active", MySqlDbType.Bit).Value = entity.Active;
            cmd.Parameters.Add("@Deleted", MySqlDbType.Bit).Value = entity.Deleted;

            if (entity.NMICompanyID == null)
            {
                cmd.ExecuteNonQuery();
                entity.NMICompanyID = id;
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("NMICompany ({0}) was not found in database.", entity.NMICompanyID));
                }
            }
        }

        public override NMICompany Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@NMICompanyID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override NMICompany Load(DataRow row)
        {
            NMICompany res = new NMICompany();

            if (!(row["NMICompanyID"] is DBNull))
                res.NMICompanyID = Convert.ToInt32(row["NMICompanyID"]);
            if (!(row["CompanyName"] is DBNull))
                res.CompanyName = Convert.ToString(row["CompanyName"]);
            if (!(row["GatewayUsername"] is DBNull))
                res.GatewayUsername = Convert.ToString(row["GatewayUsername"]);
            if (!(row["GatewayPassword"] is DBNull))
                res.GatewayPassword = Convert.ToString(row["GatewayPassword"]);
            if (!(row["GatewayIntegrated"] is DBNull))
                res.GatewayIntegrated = Convert.ToString(row["GatewayIntegrated"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);
            if (!(row["Deleted"] is DBNull))
                res.Deleted = Convert.ToBoolean(row["Deleted"]);

            return res;
        }

        private int? GetNewID(IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand q = cmdCreater.CreateCommand(@"
                select IfNull(max(NMICompanyID), 0) from NMICompany
            ");
            object res = q.ExecuteScalar();
            return Convert.ToInt32(res) + 1;
        }

    }
}
