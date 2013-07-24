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
    public partial class management_campaigns : PageX
    {
        #region Fields

        CampaignService _cs = new CampaignService();

        protected bool _showActiveCampaigns = true;
        protected bool _showActiveCampaigns2 = true;
        protected bool _showExternalCampaigns = true;
        protected bool _showExternalCampaigns2 = true;

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

            IList<CampaignDetailsView> campaigns = _cs.GetCampaignsWithDetailsOldStructure();
            IList<CampaignDetailsView> campaigns2 = _cs.GetCampaignsWithDetailsNewStructure();

            if (_showExternalCampaigns == false)
                campaigns = campaigns.Where(c => c.IsExternal == false).ToList();
            if (_showExternalCampaigns2 == false)
                campaigns2 = campaigns2.Where(c => c.IsExternal == false).ToList();

            IList<CampaignDetailsView> merchantApplications = campaigns == null ? null : campaigns.Where(c => c.IsMerchant == true && c.Active == _showActiveCampaigns).ToList();
            IList<CampaignDetailsView> merchantApplications2 = campaigns2 == null ? null : campaigns2.Where(c => c.IsMerchant == true && c.Active == _showActiveCampaigns2).ToList();

            IList<CampaignDetailsView> availableCampaigns = campaigns == null ? null : campaigns.Where(c => c.IsMerchant == false && c.Active == _showActiveCampaigns).ToList();
            IList<CampaignDetailsView> availableCampaigns2 = campaigns2 == null ? null : campaigns2.Where(c => c.IsMerchant == false && c.Active == _showActiveCampaigns2).ToList();

            IList<CampaignDetailsView> activeCampaigns = campaigns == null ? null : campaigns.Where(c => c.Active).ToList();
            IList<CampaignDetailsView> activeCampaigns2 = campaigns2 == null ? null : campaigns2.Where(c => c.Active).ToList();

            rMerchantApplications.DataSource = merchantApplications;
            rMerchantApplications2.DataSource = merchantApplications2;

            rAvailableCampaigns.DataSource = availableCampaigns;
            rAvailableCampaigns2.DataSource = availableCampaigns2;

            ddlCampaigns.DataSource = activeCampaigns;
            ddlCampaigns2.DataSource = activeCampaigns2;
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

        protected void HideArchivedCampaigns2(object sender, EventArgs e)
        {
            _showActiveCampaigns2 = true;

            base.DataBind();
        }

        protected void ShowArchivedCampaigns2(object sender, EventArgs e)
        {
            _showActiveCampaigns2 = false;

            base.DataBind();
        }

        protected void ShowExternalCampaigns(object sender, EventArgs e)
        {
            _showExternalCampaigns = true;

            base.DataBind();
        }

        protected void HideExternalCampaigns(object sender, EventArgs e)
        {
            _showExternalCampaigns = false;

            base.DataBind();
        }

        protected void ShowExternalCampaigns2(object sender, EventArgs e)
        {
            _showExternalCampaigns2 = true;

            base.DataBind();
        }

        protected void HideExternalCampaigns2(object sender, EventArgs e)
        {
            _showExternalCampaigns2 = false;

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

        protected void Duplicate2(object sender, EventArgs e)
        {
            int campaignID = int.TryParse(ddlCampaigns2.SelectedValue, out campaignID) ? campaignID : 0;

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

        protected void DoCampaignAction2(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "archive")
            {
                int campaignID = int.TryParse(e.CommandArgument as string, out campaignID) ? campaignID : -1;

                if (campaignID != -1)
                    _cs.ToggleActive(campaignID, !_showActiveCampaigns2);

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

            if(strUrl.StartsWith(http))
                return strUrl;

            else
                return http + strUrl;
        }
    }
}