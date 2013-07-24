using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignPageDataProvider : EntityDataProvider<CampaignPage>
    {
        private const string INSERT_COMMAND = "INSERT INTO CampaignPage(CampaignPageID, CampaignID, PageTypeID, HTML, Header, Title) VALUES (@campaignPageID, @campaignID, @pageTypeID, @HTML, @header, @title);";
        private const string UPDATE_COMMAND = "UPDATE CampaignPage SET CampaignID = @campaignID, PageTypeID = @PageTypeID, HTML = @html, Header = @header, Title = @title WHERE CampaignPageID = @campaignPageID;";
        private const string SELECT_COMMAND = "SELECT * FROM CampaignPage Where CampaignPageID = @campaignPageID;";
        

        public override void Save(CampaignPage entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();
            
            int? id = null;
            if (entity.CampaignPageID == 0)
            {
                id = GetNewID(cmdCreater);
                cmd.CommandText = INSERT_COMMAND;
                cmd.Parameters.Add("@CampaignPageID", MySqlDbType.Int32).Value = id;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@campaignPageID", MySqlDbType.Int32).Value = entity.CampaignPageID;
            }

            cmd.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@pageTypeID", MySqlDbType.Int32).Value = entity.PageTypeID;
            cmd.Parameters.Add("@html", MySqlDbType.Text).Value = entity.HTML;
            cmd.Parameters.Add("@header", MySqlDbType.Text).Value = entity.Header;
            cmd.Parameters.Add("@title", MySqlDbType.String).Value = entity.Title;
                 
            if (entity.CampaignPageID == 0)
            {
                cmd.ExecuteNonQuery();
                entity.CampaignPageID = id.Value;
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("CampaignPage({0}) was not found in database.", entity.CampaignPageID));
                }
            }
        }

        public override CampaignPage Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignPageID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }


        public override CampaignPage Load(DataRow row)
        {
            CampaignPage res = new CampaignPage();

            if (!(row["CampaignPageID"] is DBNull))
                res.CampaignPageID = Convert.ToInt32(row["CampaignPageID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["PageTypeID"] is DBNull))
                res.PageTypeID = Convert.ToInt32(row["PageTypeID"]);
            if (!(row["HTML"] is DBNull))
                res.HTML = Convert.ToString(row["HTML"]);
            if (!(row["Header"] is DBNull))
                res.Header = Convert.ToString(row["Header"]);
            if (!(row["Title"] is DBNull))
                res.Title = Convert.ToString(row["Title"]);

            return res;
        }

        private int? GetNewID(IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand q = cmdCreater.CreateCommand(@"
                select IfNull(max(CampaignPageID), 0) from CampaignPage
            ");
            object res = q.ExecuteScalar();
            return Convert.ToInt32(res) + 1;
        }
    }
}
