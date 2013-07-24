using System;
using TrimFuel.Web.DynamicCampaign.Logic;

using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using System.Configuration;

using MySql.Data.MySqlClient;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class _Default : BaseDynamicPage
    {
        protected override void OnInit(EventArgs e)
        {
            var cfg = ConfigurationManager.GetSection("DC.Configuration") as ConfigurationSectionHandler;
            int campaignId = 0;

            var domain = Request.Url.Host;
            if (!domain.StartsWith("www."))
            {
                domain = "www." + domain;
            }

            var schema = "http://";

            if (Request.QueryString["CampaignID"] != null)
            {
                campaignId = int.Parse(Request.QueryString["CampaignID"]);
            }
            else
            {
                if (cfg.CampaignDomains[domain] != null && cfg.CampaignDomains[domain].Domain == domain)
                    campaignId = cfg.CampaignDomains[domain].CampaignID;
                else
                {
                    var el = cfg.CampaignDomains[domain];
                    if ((el != null) && (el.Domain == domain))
                        campaignId = el.CampaignID;

                    else
                    {
                        MySqlCommand cmd = new MySqlCommand("select CampaignID from Campaign where URL = @url or Concat('www.', URL) = @url limit 1");
                        cmd.Parameters.AddWithValue("@url", domain);

                        int? campaignID = Dao.ExecuteScalar<int>(cmd);

                        if (campaignID != null)
                            campaignId = (int)campaignID;
                    }
                    Response.Write(domain);
                }
            }

            if (campaignId > 0)
                Response.Redirect(schema + Request.Url.Host + "/PreLander.aspx?CampaignID=" + campaignId + GetQuery());
            else
                Stop("Campaign ID should be set.");
        }
    }
}
