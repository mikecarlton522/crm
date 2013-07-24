using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class LeadPartnerSettingsDataProvider : EntityDataProvider<LeadPartnerSettings>
    {
        private const string INSERT_COMMAND = "INSERT INTO LeadPartnerSettings(LeadPartnerID, SetupFee, MonthlyFee, PerPourFee, SetupFeeRetail, MonthlyFeeRetail, PerPourFeeRetail) VALUES(@LeadPartnerID, @SetupFee, @MonthlyFee, @PerPourFee, @SetupFeeRetail, @MonthlyFeeRetail, @PerPourFeeRetail); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE LeadPartnerSettings SET LeadPartnerID=@LeadPartnerID, SetupFee=@SetupFee, MonthlyFee=@MonthlyFee, PerPourFee=@PerPourFee, SetupFeeRetail=@SetupFeeRetail, MonthlyFeeRetail=@MonthlyFeeRetail, PerPourFeeRetail=@PerPourFeeRetail WHERE LeadPartnerSettingID=@LeadPartnerSettingID;";
        private const string SELECT_COMMAND = "SELECT * FROM LeadPartnerSettings WHERE LeadPartnerSettingID=@LeadPartnerSettingID;";
        public override void Save(LeadPartnerSettings entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if ((entity.LeadPartnerSettingID == null) || (entity.LeadPartnerSettingID.Value == 0))
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@LeadPartnerSettingID", MySqlDbType.Int32).Value = entity.LeadPartnerSettingID;
            }

            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int64).Value = entity.LeadPartnerID;
            cmd.Parameters.Add("@SetupFee", MySqlDbType.Double).Value = entity.SetupFee;
            cmd.Parameters.Add("@SetupFeeRetail", MySqlDbType.Double).Value = entity.SetupFeeRetail;
            cmd.Parameters.Add("@MonthlyFee", MySqlDbType.Double).Value = entity.MonthlyFee;
            cmd.Parameters.Add("@MonthlyFeeRetail", MySqlDbType.Double).Value = entity.MonthlyFeeRetail;
            cmd.Parameters.Add("@PerPourFee", MySqlDbType.Double).Value = entity.PerPourFee;
            cmd.Parameters.Add("@PerPourFeeRetail", MySqlDbType.Double).Value = entity.PerPourFeeRetail;

            if ((entity.LeadPartnerSettingID == null) || (entity.LeadPartnerSettingID.Value == 0))
            {
                entity.LeadPartnerSettingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("LeadPartnerSettings ({0}) was not found in database.", entity.LeadPartnerSettingID));
                }
            }
        }

        public override LeadPartnerSettings Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@LeadPartnerSettingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override LeadPartnerSettings Load(System.Data.DataRow row)
        {
            LeadPartnerSettings res = new LeadPartnerSettings();

            if (!(row["LeadPartnerSettingID"] is DBNull))
                res.LeadPartnerSettingID = Convert.ToInt32(row["LeadPartnerSettingID"]);
            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["SetupFee"] is DBNull))
                res.SetupFee = Convert.ToDouble(row["SetupFee"]);
            if (!(row["SetupFeeRetail"] is DBNull))
                res.SetupFeeRetail = Convert.ToDouble(row["SetupFeeRetail"]);

            if (!(row["MonthlyFee"] is DBNull))
                res.MonthlyFee = Convert.ToDouble(row["MonthlyFee"]);
            if (!(row["MonthlyFeeRetail"] is DBNull))
                res.MonthlyFeeRetail = Convert.ToDouble(row["MonthlyFeeRetail"]);
            if (!(row["PerPourFee"] is DBNull))
                res.PerPourFee = Convert.ToDouble(row["PerPourFee"]);
            if (!(row["PerPourFeeRetail"] is DBNull))
                res.PerPourFeeRetail = Convert.ToDouble(row["PerPourFeeRetail"]);

            return res;
        }
    }
}
