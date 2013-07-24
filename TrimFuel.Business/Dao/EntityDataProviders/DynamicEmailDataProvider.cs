using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class DynamicEmailDataProvider : EntityDataProvider<DynamicEmail>
    {
        private const string INSERT_COMMAND = "INSERT INTO DynamicEmail(ProductID, CampaignID, Days, Content, Landing, FromName, FromAddress, Subject, Active, DynamicEmailTypeID, LandingLink) VALUES(@ProductID, @CampaignID, @Days, @Content, @Landing, @FromName, @FromAddress, @Subject, @Active, @DynamicEmailTypeID, @LandingLink); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE DynamicEmail SET ProductID=@ProductID, CampaignID=@CampaignID, Days=@Days, Content=@Content, Landing=@Landing, FromName=@FromName, FromAddress=@FromAddress, Subject=@Subject, Active=@Active, DynamicEmailTypeID=@DynamicEmailTypeID, LandingLink=@LandingLink WHERE DynamicEmailID=@DynamicEmailID;";
        private const string SELECT_COMMAND = "SELECT * FROM DynamicEmail WHERE DynamicEmailID=@DynamicEmailID;";

        public override void Save(DynamicEmail entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.DynamicEmailID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@DynamicEmailID", MySqlDbType.Int32).Value = entity.DynamicEmailID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@Days", MySqlDbType.Int16).Value = entity.Days;
            cmd.Parameters.Add("@Content", MySqlDbType.Text).Value = entity.Content;
            cmd.Parameters.Add("@Landing", MySqlDbType.Text).Value = entity.Landing;
            cmd.Parameters.Add("@FromName", MySqlDbType.VarChar).Value = entity.FromName;
            cmd.Parameters.Add("@FromAddress", MySqlDbType.VarChar).Value = entity.FromAddress;
            cmd.Parameters.Add("@Subject", MySqlDbType.VarChar).Value = entity.Subject;
            cmd.Parameters.Add("@Active", MySqlDbType.Bit).Value = entity.Active;
            cmd.Parameters.Add("@DynamicEmailTypeID", MySqlDbType.Byte).Value = entity.DynamicEmailTypeID;
            cmd.Parameters.Add("@LandingLink", MySqlDbType.VarChar).Value = entity.LandingLink;

            if (entity.DynamicEmailID == null)
            {
                entity.DynamicEmailID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("DynamicEmail({0}) was not found in database.", entity.DynamicEmailID));
                }
            }
        }

        public override DynamicEmail Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@DynamicEmailID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override DynamicEmail Load(DataRow row)
        {
            DynamicEmail res = new DynamicEmail();

            if (!(row["DynamicEmailID"] is DBNull))
                res.DynamicEmailID = Convert.ToInt32(row["DynamicEmailID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["Days"] is DBNull))
                res.Days = Convert.ToInt16(row["Days"]);
            if (!(row["Content"] is DBNull))
                res.Content = Convert.ToString(row["Content"]);
            if (!(row["Landing"] is DBNull))
                res.Landing = Convert.ToString(row["Landing"]);
            if (!(row["FromName"] is DBNull))
                res.FromName = Convert.ToString(row["FromName"]);
            if (!(row["FromAddress"] is DBNull))
                res.FromAddress = Convert.ToString(row["FromAddress"]);
            if (!(row["Subject"] is DBNull))
                res.Subject = Convert.ToString(row["Subject"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);
            if (!(row["DynamicEmailTypeID"] is DBNull))
                res.DynamicEmailTypeID = Convert.ToByte(row["DynamicEmailTypeID"]);
            if (!(row["LandingLink"] is DBNull))
                res.LandingLink = Convert.ToString(row["LandingLink"]);

            return res;
        }
    }
}
