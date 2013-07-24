using System;
using System.Collections.Generic;

using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Web.Screenshots;
using TrimFuel.Business.Utils;
using System.Web.UI.WebControls;

namespace TrimFuel.Web.Admin
{
    public partial class edit_campaigns : PageX
    {
        private CampaignService _cs = new CampaignService();

        private Campaign _campaign = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddParentCampaign.DataSource = _cs.GetCampaignsList();

                int campaignID = int.TryParse(Request["campaignID"], out campaignID) ? campaignID : -1;

                if (campaignID > 0)
                {
                    _campaign = _cs.GetCampaignByID(campaignID);

                    Fill(_campaign);
                }
            }
        }

        protected void Save(object sender, EventArgs e)
        {
            int campaignID = 0;

            int subscriptionID = int.TryParse(Request["subscription"], out subscriptionID) ? subscriptionID : 0;

            bool newCampaign = !int.TryParse(Request["campaignID"], out campaignID);

            Campaign campaign = newCampaign ? new Campaign() : _cs.GetCampaignByID(campaignID);

            if (campaign == null)
                return;

            else
            {
                campaign.DisplayName = txtCampaignName.Text;
                campaign.URL = txtCampaignURL.Text;
                //campaign.Redirect = cbRedirect.Checked;
                campaign.RedirectURL = ddlRedirectURL.SelectedValue;
                campaign.IsMerchant = cbMerchant.Checked;
                campaign.IsSave = cbSave.Checked;
                campaign.IsExternal = cbExternal.Checked;
                campaign.Active = true;

                if (subscriptionID == 0)
                {
                    //subscription must be selected: show error message
                    statusMessage.ForeColor = System.Drawing.Color.Red;
                    statusMessage.Text = "Please select a subscription before saving.";

                    return;
                }
                else
                    campaign.SubscriptionID = subscriptionID;

                try
                {
                    campaign.ParentCampaignID = Convert.ToInt32(ddParentCampaign.SelectedValue);
                }
                catch { }

                if (_cs.Save<Campaign>(campaign))
                {
                    if (!string.IsNullOrEmpty(txtPreLanderPageHTML.Text))
                        _cs.SaveCampaignPage(txtPreLanderHeaderHTML.Text, txtPreLanderPageHTML.Text, txtPreLanderTitle.Text, (int)campaign.CampaignID, PageTypeEnum.PreLander);
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.PreLander);

                    if (!string.IsNullOrEmpty(txtLandingPageHTML.Text))
                        _cs.SaveCampaignPage(txtLandingHeaderHTML.Text, txtLandingPageHTML.Text, txtLandingTitle.Text, (int)campaign.CampaignID, PageTypeEnum.Landing);
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Landing);

                    if (!string.IsNullOrEmpty(txtBillingPageHTML.Text))
                        _cs.SaveCampaignPage(txtBillingHeaderHTML.Text, txtBillingPageHTML.Text, txtBillingTitle.Text, (int)campaign.CampaignID, PageTypeEnum.Billing);
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Billing);

                    if (!string.IsNullOrEmpty(txtUpsell1PageHTML.Text))
                    {
                        var res = _cs.SaveCampaignPage(txtUpsell1HeaderHTML.Text, txtUpsell1PageHTML.Text, txtUpsell1Title.Text, (int)campaign.CampaignID, PageTypeEnum.Upsell_1);
                        if (res.State == BusinessErrorState.Success)
                            _cs.SaveCampaignUpsell(res.ReturnValue.CampaignPageID, txtUpsell1Code.Text, Utility.TryGetInt(txtUpsell1Quantity.Text), Utility.TryGetDecimal(txtUpsell1Price.Text), Utility.TryGetInt(ddlSubscriptionUpsell1.SelectedValue), cbUpsell1PlanChange.Checked, null);
                    }
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Upsell_1);

                    if (!string.IsNullOrEmpty(txtUpsell2PageHTML.Text))
                    {
                        var res = _cs.SaveCampaignPage(txtUpsell2HeaderHTML.Text, txtUpsell2PageHTML.Text, txtUpsell2Title.Text, (int)campaign.CampaignID, PageTypeEnum.Upsell_2);
                        if (res.State == BusinessErrorState.Success)
                            _cs.SaveCampaignUpsell(res.ReturnValue.CampaignPageID, txtUpsell2Code.Text, Utility.TryGetInt(txtUpsell2Quantity.Text), Utility.TryGetDecimal(txtUpsell2Price.Text), Utility.TryGetInt(ddlSubscriptionUpsell2.SelectedValue), cbUpsell2PlanChange.Checked, null);
                    }
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Upsell_2);

                    if (!string.IsNullOrEmpty(txtUpsell3PageHTML.Text))
                    {
                        var res = _cs.SaveCampaignPage(txtUpsell3HeaderHTML.Text, txtUpsell3PageHTML.Text, txtUpsell3Title.Text, (int)campaign.CampaignID, PageTypeEnum.Upsell_3);
                        if (res.State == BusinessErrorState.Success)
                            _cs.SaveCampaignUpsell(res.ReturnValue.CampaignPageID, txtUpsell3Code.Text, Utility.TryGetInt(txtUpsell3Quantity.Text), Utility.TryGetDecimal(txtUpsell3Price.Text), Utility.TryGetInt(ddlSubscriptionUpsell3.SelectedValue), cbUpsell3PlanChange.Checked, null);
                    }
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Upsell_3);

                    if (!string.IsNullOrEmpty(txtConfirmationPageHTML.Text))
                        _cs.SaveCampaignPage(txtConfirmationHeaderHTML.Text, txtConfirmationPageHTML.Text, txtConfirmationTitle.Text, (int)campaign.CampaignID, PageTypeEnum.Confirmation);
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Confirmation);

                    statusMessage.ForeColor = System.Drawing.Color.Green;
                    statusMessage.Text = "Your changes have been saved.";
                }
                else
                {
                    statusMessage.ForeColor = System.Drawing.Color.Red;
                    statusMessage.Text = "An error occured while saving your changes. Please try again.";
                }
            }

            if (!newCampaign)
                CreateScreenshot((int)campaign.CampaignID, 300, 200);

            Fill(campaign);
            DataBind();
        }

        public override string HeaderString
        {
            get
            {
                if (_campaign != null)
                    return string.Format("Edit Campaign: {0}", _campaign.DisplayName);

                return "Create New Campaign";
            }
        }

        private void Fill(Campaign campaign)
        {
            txtCampaignName.Text = campaign.DisplayName;

            txtCampaignURL.Text = campaign.URL;

            //cbRedirect.Checked = campaign.Redirect;
            if (!string.IsNullOrEmpty(campaign.RedirectURL))
                ddlRedirectURL.SelectedValue = campaign.RedirectURL;

            cbMerchant.Checked = campaign.IsMerchant;

            cbSave.Checked = campaign.IsSave;

            cbExternal.Checked = campaign.IsExternal;

            if (campaign.ParentCampaignID != null)
                ddParentCampaign.SelectedValue = campaign.ParentCampaignID.ToString();

            IList<CampaignPage> campaignPages = _cs.GetCampaignPages((int)campaign.CampaignID);

            foreach (CampaignPage campaignPage in campaignPages)
            {
                switch (campaignPage.PageTypeID)
                {
                    case PageTypeEnum.PreLander:
                        txtPreLanderTitle.Text = campaignPage.Title;
                        txtPreLanderHeaderHTML.Text = campaignPage.Header;
                        txtPreLanderPageHTML.Text = campaignPage.HTML;
                        break;

                    case PageTypeEnum.Landing:
                        txtLandingTitle.Text = campaignPage.Title;
                        txtLandingHeaderHTML.Text = campaignPage.Header;
                        txtLandingPageHTML.Text = campaignPage.HTML;
                        break;

                    case PageTypeEnum.Billing:
                        txtBillingTitle.Text = campaignPage.Title;
                        txtBillingHeaderHTML.Text = campaignPage.Header;
                        txtBillingPageHTML.Text = campaignPage.HTML;
                        break;

                    case PageTypeEnum.Upsell_1:
                        txtUpsell1Title.Text = campaignPage.Title;
                        txtUpsell1HeaderHTML.Text = campaignPage.Header;
                        txtUpsell1PageHTML.Text = campaignPage.HTML;
                        //TODO: load other upsell fields
                        FillUpsellAdditional(campaignPage.CampaignPageID, txtUpsell1Code, txtUpsell1Price, txtUpsell1Quantity, ddlSubscriptionUpsell1, cbUpsell1PlanChange);
                        break;

                    case PageTypeEnum.Upsell_2:
                        txtUpsell2Title.Text = campaignPage.Title;
                        txtUpsell2HeaderHTML.Text = campaignPage.Header;
                        txtUpsell2PageHTML.Text = campaignPage.HTML;
                        //TODO: load other upsell fields
                        FillUpsellAdditional(campaignPage.CampaignPageID, txtUpsell2Code, txtUpsell2Price, txtUpsell2Quantity, ddlSubscriptionUpsell2, cbUpsell2PlanChange);                        
                        break;

                    case PageTypeEnum.Upsell_3:
                        txtUpsell3Title.Text = campaignPage.Title;
                        txtUpsell3HeaderHTML.Text = campaignPage.Header;
                        txtUpsell3PageHTML.Text = campaignPage.HTML;
                        //TODO: load other upsell fields
                        FillUpsellAdditional(campaignPage.CampaignPageID, txtUpsell3Code, txtUpsell3Price, txtUpsell3Quantity, ddlSubscriptionUpsell3, cbUpsell3PlanChange);
                        break;

                    case PageTypeEnum.Confirmation:
                        txtConfirmationTitle.Text = campaignPage.Title;
                        txtConfirmationHeaderHTML.Text = campaignPage.Header;
                        txtConfirmationPageHTML.Text = campaignPage.HTML;
                        break;
                }
            }
        }

        private void CreateScreenshot(int campaignID, int width, int height)
        {
            string client = Config.Current.APPLICATION_ID.Split('.')[0];

            string path = "C:/web/dashboard/images/campaign-screenshots/" + client;

            if (Config.Current.APPLICATION_ID.Contains("localhost"))
                return;

            try
            {
                WebShot.GenerateScreenshot(campaignID, width, height, path);
            }
            catch { }
        }

        private void FillUpsellAdditional(int campaignPageID, TextBox txtUpsellCode, TextBox txtUpsellPrice, TextBox txtUpsellQuantity, DropDownList ddlSubscriptionUpsell, CheckBox cbUpsellPlanChange)
        {
            cbUpsellPlanChange.Checked = false;
            txtUpsellCode.Text = string.Empty;
            txtUpsellPrice.Text = string.Empty;
            txtUpsellQuantity.Text = string.Empty;
            ddlSubscriptionUpsell.SelectedIndex = 0;
            var campaignUpsell = _cs.GetCampaignUpsell(campaignPageID);
            if (campaignUpsell != null)
            {
                cbUpsell3PlanChange.Checked = false;
                txtUpsellCode.Text = campaignUpsell.ProductCode;
                txtUpsellPrice.Text = Utility.FormatPrice(campaignUpsell.Price);
                txtUpsellQuantity.Text = campaignUpsell.Quantity.ToString();
                if (campaignUpsell.SubscriptionID != null)
                {
                    cbUpsellPlanChange.Checked = true;
                    ddlSubscriptionUpsell.SelectedValue = campaignUpsell.SubscriptionID.ToString();
                }
            }
        }
    }
}