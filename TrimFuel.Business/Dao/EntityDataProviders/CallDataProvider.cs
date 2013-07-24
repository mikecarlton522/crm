using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CallDataProvider : EntityDataProvider<Call>
    {
        private const string INSERT_COMMAND = "INSERT INTO `Call`(ExternalCallID, StartDT, EndDT, ANI, DNIS, AgentID, AgentName, AgentLocation, DispositionID, DispositionName, AgentNotes, CustomerID, CustomerProduct, CreateDT, Partner, Version, HoldTime) VALUES(@ExternalCallID, @StartDT, @EndDT, @ANI, @DNIS, @AgentID, @AgentName, @AgentLocation, @DispositionID, @DispositionName, @AgentNotes, @CustomerID, @CustomerProduct, @CreateDT, @Partner, @Version, @HoldTime);";
        private const string UPDATE_COMMAND = "UPDATE `Call` SET ExternalCallID=@ExternalCallID, StartDT=@StartDT, EndDT=@EndDT, ANI=@ANI, DNIS=@DNIS, AgentID=@AgentID, AgentName=@AgentName, AgentLocation=@AgentLocation, DispositionID=@DispositionID, DispositionName=@DispositionName, AgentNotes=@AgentNotes, CustomerID=@CustomerID, CustomerProduct=@CustomerProduct, CreateDT=@CreateDT, Partner=@Partner, Version=@Version, HoldTime=@HoldTime WHERE CallID=@CallID;";
        private const string SELECT_COMMAND = "SELECT * FROM `Call` WHERE CallID=@CallID;";

        public override void Save(Call entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CallID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CallID", MySqlDbType.Int64).Value = entity.CallID;
            }

            cmd.Parameters.Add("@ExternalCallID", MySqlDbType.Int64).Value = entity.ExternalCallID;
            cmd.Parameters.Add("@StartDT", MySqlDbType.Timestamp).Value = entity.StartDT;
            cmd.Parameters.Add("@EndDT", MySqlDbType.Timestamp).Value = entity.EndDT;
            cmd.Parameters.Add("@ANI", MySqlDbType.VarChar).Value = entity.ANI;
            cmd.Parameters.Add("@DNIS", MySqlDbType.VarChar).Value = entity.DNIS;
            cmd.Parameters.Add("@AgentID", MySqlDbType.Int32).Value = entity.AgentID;
            cmd.Parameters.Add("@AgentName", MySqlDbType.VarChar).Value = entity.AgentName;
            cmd.Parameters.Add("@AgentLocation", MySqlDbType.VarChar).Value = entity.AgentLocation;
            cmd.Parameters.Add("@DispositionID", MySqlDbType.Int32).Value = entity.DispositionID;
            cmd.Parameters.Add("@DispositionName", MySqlDbType.VarChar).Value = entity.DispositionName;
            cmd.Parameters.Add("@AgentNotes", MySqlDbType.VarChar).Value = entity.AgentNotes;
            cmd.Parameters.Add("@CustomerID", MySqlDbType.Int64).Value = entity.CustomerID;
            cmd.Parameters.Add("@CustomerProduct", MySqlDbType.VarChar).Value = entity.CustomerProduct;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Partner", MySqlDbType.VarChar).Value = entity.Partner;
            cmd.Parameters.Add("@Version", MySqlDbType.VarChar).Value = entity.Version;
            cmd.Parameters.Add("@HoldTime", MySqlDbType.Int32).Value = entity.HoldTime;

            if (entity.CallID == null)
            {
                entity.CallID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Call({0}) was not found in database.", entity.CallID));
                }
            }
        }

        public override Call Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CallID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Call Load(DataRow row)
        {
            Call res = new Call();

            if (!(row["CallID"] is DBNull))
                res.CallID = Convert.ToInt64(row["CallID"]);
            if (!(row["ExternalCallID"] is DBNull))
                res.ExternalCallID = Convert.ToInt64(row["ExternalCallID"]);
            if (!(row["StartDT"] is DBNull))
                res.StartDT = Convert.ToDateTime(row["StartDT"]);
            if (!(row["EndDT"] is DBNull))
                res.EndDT = Convert.ToDateTime(row["EndDT"]);
            if (!(row["ANI"] is DBNull))
                res.ANI = Convert.ToString(row["ANI"]);
            if (!(row["DNIS"] is DBNull))
                res.DNIS = Convert.ToString(row["DNIS"]);
            if (!(row["AgentID"] is DBNull))
                res.AgentID = Convert.ToInt32(row["AgentID"]);
            if (!(row["AgentName"] is DBNull))
                res.AgentName = Convert.ToString(row["AgentName"]);
            if (!(row["AgentLocation"] is DBNull))
                res.AgentLocation = Convert.ToString(row["AgentLocation"]);
            if (!(row["DispositionID"] is DBNull))
                res.DispositionID = Convert.ToInt32(row["DispositionID"]);
            if (!(row["DispositionName"] is DBNull))
                res.DispositionName = Convert.ToString(row["DispositionName"]);
            if (!(row["AgentNotes"] is DBNull))
                res.AgentNotes = Convert.ToString(row["AgentNotes"]);
            if (!(row["CustomerID"] is DBNull))
                res.CustomerID = Convert.ToInt64(row["CustomerID"]);
            if (!(row["CustomerProduct"] is DBNull))
                res.CustomerProduct = Convert.ToString(row["CustomerProduct"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Partner"] is DBNull))
                res.Partner = Convert.ToString(row["Partner"]);
            if (!(row["Version"] is DBNull))
                res.Version = Convert.ToString(row["Version"]);
            if (!(row["HoldTime"] is DBNull))
                res.HoldTime = Convert.ToInt32(row["HoldTime"]);

            return res;
        }
    }
}
