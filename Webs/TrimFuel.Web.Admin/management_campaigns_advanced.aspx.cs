using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;

namespace TrimFuel.Web.Admin
{
    public partial class management_campaigns_advanced : PageX
    {
        #region Fields

        CampaignService _cs = new CampaignService();

        protected bool _showActiveCampaigns = true;

        #endregion

        #region Properties

        public override string HeaderString
        {
            get { return "Campaign Manager"; }
        }

        #endregion

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<CampaignDetailsView> campaigns = _cs.GetCampaignsWithDetailsNewStructure();

            IList<CampaignDetailsView> merchantApplications = campaigns.Where(c => c.IsMerchant == true && c.Active == _showActiveCampaigns).ToList();

            IList<CampaignDetailsView> availableCampaigns = campaigns.Where(c => c.IsMerchant == false && c.Active == _showActiveCampaigns).ToList();

            IList<CampaignDetailsView> activeCampaigns = campaigns.Where(c => c.Active).ToList();

            rMerchantApplications.DataSource = merchantApplications;

            rAvailableCampaigns.DataSource = availableCampaigns;

            ddlCampaigns.DataSource = activeCampaigns;
        }

        protected void HideArchivedCampaigns(object sender, EventArgs e)
        {
            _showActiveCampaigns = true;

            base.DataBind();
        }

        protected void ShowArchivedCampaigns(object sender, EventArgs e)
        {
            _showActiveCampaigns = false;

            base.DataBind();
        }

        protected void Duplicate(object sender, EventArgs e)
        {
            int campaignID = int.TryParse(ddlCampaigns.SelectedValue, out campaignID) ? campaignID : 0;

            if (campaignID > 0)
            {
                new CampaignService().CreateDuplicateCampaign(campaignID);
            }

            base.DataBind();
        }

        protected void DoCampaignAction(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "archive")
            {
                int campaignID = int.TryParse(e.CommandArgument as string, out campaignID) ? campaignID : -1;

                if (campaignID != -1)
                    _cs.ToggleActive(campaignID, !_showActiveCampaigns);

                base.DataBind();
            }
        }

        protected string GetScreenshot(int campaignID)
        {
            string client = Config.Current.APPLICATION_ID.Split('.')[0];

            string path = "C:/Web/Dashboard/images/campaign-screenshots/" + client;

            string file = string.Format("{0}/thumbnail-{1}_300x200.jpg", path, campaignID);

            if (File.Exists(file))
                return string.Format("/images/campaign-screenshots/{0}/thumbnail-{1}_300x200.jpg", client, campaignID);

            else
                return string.Format("/images/campaign-screenshots/{0}/dummy.jpg", client);
        }

        protected string GetURL(object url)
        {
            if (url == null)
                return null;

            string strUrl = url.ToString();

            string http = "http://";

            if (strUrl.StartsWith(http))
                return strUrl;

            else
                return http + strUrl;
        }
    }
}