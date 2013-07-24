using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AssertigyMIDDailyCapDataProvider : EntityDataProvider<AssertigyMIDDailyCap>
    {
        private const string INSERT_COMMAND = "INSERT INTO AssertigyMIDDailyCap(AssertigyMIDID, TotalAmount, CreateDT) VALUES(@AssertigyMIDID, @TotalAmount, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE AssertigyMIDDailyCap SET AssertigyMIDID = @AssertigyMIDID, TotalAmount = @TotalAmount, CreateDT = @CreateDT WHERE AssertigyMIDDailyCapID=@AssertigyMIDDailyCapID;";
        private const string SELECT_COMMAND = "SELECT * FROM AssertigyMIDDailyCap WHERE AssertigyMIDDailyCapID = @AssertigyMIDDailyCapID;";

        public override void Save(AssertigyMIDDailyCap entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AssertigyMIDDailyCapID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@AssertigyMIDDailyCapID", MySqlDbType.Int32).Value = entity.AssertigyMIDDailyCapID;
            }

            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;
            cmd.Parameters.Add("@TotalAmount", MySqlDbType.Double).Value = entity.TotalAmount;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.AssertigyMIDDailyCapID == null)
            {
                entity.AssertigyMIDDailyCapID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("AssertigyMIDDailyCap({0}) was not found in database.", entity.AssertigyMIDDailyCapID));
                }
            }
        }

        public override AssertigyMIDDailyCap Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AssertigyMIDDailyCapID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AssertigyMIDDailyCap Load(DataRow row)
        {
            AssertigyMIDDailyCap res = new AssertigyMIDDailyCap();

            if (!(row["AssertigyMIDDailyCapID"] is DBNull))
                res.AssertigyMIDDailyCapID = Convert.ToInt32(row["AssertigyMIDDailyCapID"]);
            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["TotalAmount"] is DBNull))
                res.TotalAmount = Convert.ToDouble(row["TotalAmount"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
