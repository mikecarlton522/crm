using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TPClientDataProvider : EntityDataProvider<TPClient>
    {
        private const string INSERT_COMMAND = "INSERT INTO TPClient(Name, TPModeID, Username, Password, DomainName, ConnectionStringDotNET, PostageAllowed) VALUES(@Name, @TPModeID, @Username, @Password, @DomainName, @ConnectionStringDotNET, @PostageAllowed); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE TPClient SET Name=@Name, TPModeID=@TPModeID, Username=@Username, Password=@Password, DomainName=@DomainName, ConnectionStringDotNET=@ConnectionStringDotNET, TriangleFulfillment=@TriangleFulfillment, TriangleCRM=@TriangleCRM, TelephonyServices=@TelephonyServices, CallCenterServices=@CallCenterServices, TechnologyServices=@TechnologyServices, MediaServices=@MediaServices, PostageAllowed=@PostageAllowed WHERE TPClientID=@TPClientID;";
        private const string SELECT_COMMAND = "SELECT * FROM TPClient WHERE TPClientID=@TPClientID;";

        public override void Save(TPClient entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TPClientID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TPClientID", MySqlDbType.Int32).Value = entity.TPClientID;
            }

            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;
            cmd.Parameters.Add("@TPModeID", MySqlDbType.Int32).Value = entity.TPModeID;
            cmd.Parameters.Add("@Username", MySqlDbType.VarChar).Value = entity.Username;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;
            cmd.Parameters.Add("@DomainName", MySqlDbType.VarChar).Value = entity.DomainName;
            cmd.Parameters.Add("@ConnectionStringDotNET", MySqlDbType.VarChar).Value = entity.ConnectionString;
            cmd.Parameters.Add("@TriangleFulfillment", MySqlDbType.Bit).Value = entity.TriangleFulfillment;
            cmd.Parameters.Add("@TriangleCRM", MySqlDbType.Bit).Value = entity.TriangleCRM;
            cmd.Parameters.Add("@TelephonyServices", MySqlDbType.Bit).Value = entity.TelephonyServices;
            cmd.Parameters.Add("@CallCenterServices", MySqlDbType.Bit).Value = entity.CallCenterServices;
            cmd.Parameters.Add("@TechnologyServices", MySqlDbType.Bit).Value = entity.TechnologyServices;
            cmd.Parameters.Add("@MediaServices", MySqlDbType.Bit).Value = entity.MediaServices;
            cmd.Parameters.Add("@PostageAllowed", MySqlDbType.Decimal).Value = entity.PostageAllowed;

            if (entity.TPClientID == null)
            {
                entity.TPClientID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("TPClient({0}) was not found in database.", entity.TPClientID));
                }
            }
        }

        public override TPClient Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TPClientID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override TPClient Load(DataRow row)
        {
            TPClient res = new TPClient();

            if (!(row["TPClientID"] is DBNull))
                res.TPClientID = Convert.ToInt32(row["TPClientID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["TPModeID"] is DBNull))
                res.TPModeID = Convert.ToInt32(row["TPModeID"]);
            if (!(row["Username"] is DBNull))
                res.Username = Convert.ToString(row["Username"]);
            if (!(row["Password"] is DBNull))
                res.Password = Convert.ToString(row["Password"]);
            if (!(row["DomainName"] is DBNull))
                res.DomainName = Convert.ToString(row["DomainName"]);
            if (!(row["ConnectionStringDotNET"] is DBNull))
                res.ConnectionString = Convert.ToString(row["ConnectionStringDotNET"]);
            if (!(row["TriangleFulfillment"] is DBNull))
                res.TriangleFulfillment = Convert.ToBoolean(row["TriangleFulfillment"]);
            if (!(row["TriangleCRM"] is DBNull))
                res.TriangleCRM = Convert.ToBoolean(row["TriangleCRM"]);
            if (!(row["TelephonyServices"] is DBNull))
                res.TelephonyServices = Convert.ToBoolean(row["TelephonyServices"]);
            if (!(row["CallCenterServices"] is DBNull))
                res.CallCenterServices = Convert.ToBoolean(row["CallCenterServices"]);
            if (!(row["TechnologyServices"] is DBNull))
                res.TechnologyServices = Convert.ToBoolean(row["TechnologyServices"]);
            if (!(row["MediaServices"] is DBNull))
                res.MediaServices = Convert.ToBoolean(row["MediaServices"]);
            if (!(row["PostageAllowed"] is DBNull))
                res.PostageAllowed = Convert.ToDecimal(row["PostageAllowed"]);
            return res;
        }
    }
}
