using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignDataProvider : EntityDataProvider<Campaign>
    {
        private const string INSERT_COMMAND = "INSERT INTO Campaign(CampaignID, DisplayName, SubscriptionID, Percentage, Active, CreateDT, Redirect, IsSave, ParentCampaignID, EnableFitFactory, URL, IsSTO, SendUserEmail, IsMerchant, IsRiskScoring, IsDupeChecking, IsExternal, ShipperID, RedirectURL) VALUES(@CampaignID, @DisplayName, @SubscriptionID, @Percentage, @Active, @CreateDT, @Redirect, @IsSave, @ParentCampaignID, @EnableFitFactory, @URL, @IsSTO, @SendUserEmail, @IsMerchant, @IsRiskScoring, @IsDupeChecking, @IsExternal, @ShipperID, @RedirectURL);";
        private const string UPDATE_COMMAND = "UPDATE Campaign SET CampaignID=@CampaignID, DisplayName=@DisplayName, SubscriptionID=@SubscriptionID, Percentage=@Percentage, Active=@Active, CreateDT=@CreateDT, Redirect=@Redirect, IsSave=@IsSave, ParentCampaignID=@ParentCampaignID, EnableFitFactory=@EnableFitFactory, URL=@URL, IsSTO=@IsSTO, SendUserEmail=@SendUserEmail, IsMerchant=@IsMerchant, IsRiskScoring=@IsRiskScoring, IsDupeChecking=@IsDupeChecking, IsExternal=@IsExternal, ShipperID=@ShipperID, RedirectURL=@RedirectURL WHERE CampaignID=@CampaignID;";
        private const string SELECT_COMMAND = "SELECT * FROM Campaign WHERE CampaignID=@CampaignID;";

        public override void Save(Campaign entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            int? id = null;
            if (entity.CampaignID == null)
            {
                id = GetNewID(cmdCreater);
                cmd.CommandText = INSERT_COMMAND;
                cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = id;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            }

            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = entity.SubscriptionID;
            cmd.Parameters.Add("@Percentage", MySqlDbType.Int32).Value = entity.Percentage;
            cmd.Parameters.Add("@Active", MySqlDbType.Bit).Value = entity.Active;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Redirect", MySqlDbType.Bit).Value = entity.Redirect;
            cmd.Parameters.Add("@IsSave", MySqlDbType.Bit).Value = entity.IsSave;
            cmd.Parameters.Add("@ParentCampaignID", MySqlDbType.Int32).Value = entity.ParentCampaignID;
            cmd.Parameters.Add("@EnableFitFactory", MySqlDbType.Int32).Value = entity.EnableFitFactory;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URL;
            cmd.Parameters.Add("@IsSTO", MySqlDbType.Bit).Value = entity.IsSTO;
            cmd.Parameters.Add("@SendUserEmail", MySqlDbType.Bit).Value = entity.SendUserEmail;
            cmd.Parameters.Add("@IsMerchant", MySqlDbType.Bit).Value = entity.IsMerchant;
            cmd.Parameters.Add("@IsRiskScoring", MySqlDbType.Bit).Value = entity.IsRiskScoring;
            cmd.Parameters.Add("@IsDupeChecking", MySqlDbType.Bit).Value = entity.IsDupeChecking;
            cmd.Parameters.Add("@IsExternal", MySqlDbType.Bit).Value = entity.IsExternal;
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = entity.ShipperID;
            cmd.Parameters.Add("@RedirectURL", MySqlDbType.VarChar).Value = entity.RedirectURL;

            if (entity.CampaignID == null)
            {
                cmd.ExecuteNonQuery();
                entity.CampaignID = id;
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Campaign({0}) was not found in database.", entity.CampaignID));
                }
            }
        }

        public override Campaign Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Campaign Load(DataRow row)
        {
            Campaign res = new Campaign();

            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt32(row["Percentage"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Redirect"] is DBNull))
                res.Redirect = Convert.ToBoolean(row["Redirect"]);
            if (!(row["IsSave"] is DBNull))
                res.IsSave = Convert.ToBoolean(row["IsSave"]);
            if (!(row["ParentCampaignID"] is DBNull))
                res.ParentCampaignID = Convert.ToInt32(row["ParentCampaignID"]);
            if (!(row["EnableFitFactory"] is DBNull))
                res.EnableFitFactory = Convert.ToBoolean(row["EnableFitFactory"]);
            if (!(row["URL"] is DBNull))
                res.URL = Convert.ToString(row["URL"]);
            if (!(row["IsSTO"] is DBNull))
                res.IsSTO = Convert.ToBoolean(row["IsSTO"]);
            if (!(row["SendUserEmail"] is DBNull))
                res.SendUserEmail = Convert.ToBoolean(row["SendUserEmail"]);
            if (!(row["IsMerchant"] is DBNull))
                res.IsMerchant = Convert.ToBoolean(row["IsMerchant"]);
            if (!(row["IsRiskScoring"] is DBNull))
                res.IsRiskScoring = Convert.ToBoolean(row["IsRiskScoring"]);
            if (!(row["IsDupeChecking"] is DBNull))
                res.IsDupeChecking = Convert.ToBoolean(row["IsDupeChecking"]);
            if (!(row["IsExternal"] is DBNull))
                res.IsExternal = Convert.ToBoolean(row["IsExternal"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["RedirectURL"] is DBNull))
                res.RedirectURL = Convert.ToString(row["RedirectURL"]);

            return res;
        }

        private int? GetNewID(IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand q = cmdCreater.CreateCommand(@"
                select IfNull(max(CampaignID), 0) from Campaign
            ");
            object res = q.ExecuteScalar();
            return Convert.ToInt32(res) + 1;
        }
    }
}
