using System;
using System.Collections.Generic;
using System.Linq;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Web.Screenshots;
using TrimFuel.Business.Utils;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Controls.SubscriptionControl;

namespace TrimFuel.Web.Admin
{
    public partial class edit_campaigns_advanced : PageX
    {
        private CampaignService _cs = new CampaignService();
        private SubscriptionNewService subsService = new SubscriptionNewService();

        private Campaign _campaign = null;

        public int? CampaignID
        {
            get
            {
                return Utility.TryGetInt(hdnCampaignID.Value) ?? Utility.TryGetInt(Request["campaignID"]);
            }
            set
            {
                if (value == null)
                {
                    hdnCampaignID.Value = "";
                }
                else
                {
                    hdnCampaignID.Value = value.ToString();
                }
            }
        }

        //public int? CampaignSubscriptionID
        //{
        //    get { return ViewState["CampaignSubscriptionID"] != null ? (int?)ViewState["CampaignSubscriptionID"] : null; }
        //    set { ViewState["CampaignSubscriptionID"] = value; }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddParentCampaign.DataSource = _cs.GetCampaignsList();

                if (CampaignID != null)
                {
                    _campaign = _cs.GetCampaignByID(CampaignID.Value);

                    Fill(_campaign);
                }
                else
                {
                    hdnSubscriptionTab.Value = "0";
                }
            }
            else
            {
                //CampaignSubscriptionID = Utility.TryGetInt(Request["subscription"]);
            }
        }

        protected void Save(object sender, EventArgs e)
        {
            //int subscriptionID = int.TryParse(Request["subscription"], out subscriptionID) ? subscriptionID : 1;

            Campaign campaign = (CampaignID == null ? new Campaign() : _cs.GetCampaignByID(CampaignID.Value));

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
                campaign.Active = true;

                if (subscription1.SelectedRecurringPlanID == null)
                {
                    statusMessage.ForeColor = System.Drawing.Color.Red;
                    statusMessage.Text = "Please select a subscription before saving.";

                    return;
                }
                else
                {
                    string subscriptionError = subscription1.Validate();
                    if (!string.IsNullOrEmpty(subscriptionError))
                    {
                        statusMessage.ForeColor = System.Drawing.Color.Red;
                        statusMessage.Text = subscriptionError;

                        return;
                    }
                }
                //subscription must be selected: show error message
                //if (subscriptionID == 0)
                //{
                //    statusMessage.ForeColor = System.Drawing.Color.Red;
                //    statusMessage.Text = "Please select a subscription before saving.";

                //    return;
                //}
                //else
                //{
                //    campaign.SubscriptionID = (subscriptionID != 0 ? new int?(subscriptionID) : null);
                //}
                //set default product group subscription
                //if (campaign.SubscriptionID == null)
                //{
                RecurringPlan plan = subsService.Load<RecurringPlan>(subscription1.SelectedRecurringPlanID);
                if (plan == null || plan.ProductID == null)
                {
                    statusMessage.ForeColor = System.Drawing.Color.Red;
                    statusMessage.Text = "An error occured while saving your changes. Please try again.";

                    return;
                }
                var defaultProductSubscription = new ProductService().GetOrCreteDefaultProductSubscription(plan.ProductID.Value);
                if (defaultProductSubscription == null)
                {
                    statusMessage.ForeColor = System.Drawing.Color.Red;
                    statusMessage.Text = "An error occured while saving your changes. Please try again.";

                    return;
                }
                campaign.SubscriptionID = defaultProductSubscription.SubscriptionID;
                //}

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
                            _cs.SaveCampaignUpsell(res.ReturnValue.CampaignPageID, txtUpsell1Code.Text, Utility.TryGetInt(txtUpsell1Quantity.Text), Utility.TryGetDecimal(txtUpsell1Price.Text), null, cbUpsell1PlanChange.Checked, CampaignSubscriptionUpsell1.SelectedRecurringPlanID);
                    }
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Upsell_1);

                    if (!string.IsNullOrEmpty(txtUpsell2PageHTML.Text))
                    {
                        var res = _cs.SaveCampaignPage(txtUpsell2HeaderHTML.Text, txtUpsell2PageHTML.Text, txtUpsell2Title.Text, (int)campaign.CampaignID, PageTypeEnum.Upsell_2);
                        if (res.State == BusinessErrorState.Success)
                            _cs.SaveCampaignUpsell(res.ReturnValue.CampaignPageID, txtUpsell2Code.Text, Utility.TryGetInt(txtUpsell2Quantity.Text), Utility.TryGetDecimal(txtUpsell2Price.Text), null, cbUpsell2PlanChange.Checked, CampaignSubscriptionUpsell2.SelectedRecurringPlanID);
                    }
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Upsell_2);

                    if (!string.IsNullOrEmpty(txtUpsell3PageHTML.Text))
                    {
                        var res = _cs.SaveCampaignPage(txtUpsell3HeaderHTML.Text, txtUpsell3PageHTML.Text, txtUpsell3Title.Text, (int)campaign.CampaignID, PageTypeEnum.Upsell_3);
                        if (res.State == BusinessErrorState.Success)
                            _cs.SaveCampaignUpsell(res.ReturnValue.CampaignPageID, txtUpsell3Code.Text, Utility.TryGetInt(txtUpsell3Quantity.Text), Utility.TryGetDecimal(txtUpsell3Price.Text), null, cbUpsell3PlanChange.Checked, CampaignSubscriptionUpsell3.SelectedRecurringPlanID);
                    }
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Upsell_3);

                    if (!string.IsNullOrEmpty(txtConfirmationPageHTML.Text))
                        _cs.SaveCampaignPage(txtConfirmationHeaderHTML.Text, txtConfirmationPageHTML.Text, txtConfirmationTitle.Text, (int)campaign.CampaignID, PageTypeEnum.Confirmation);
                    else
                        _cs.DeleteCampaignPage((int)campaign.CampaignID, PageTypeEnum.Confirmation);

                    subsService.UpdateCampaignSubscription(campaign.CampaignID.Value, subscription1.SelectedRecurringPlanID,
                        subscription1.SelectedTrialPrice, subscription1.SelectedTrialPeriod, subscription1.SelectedInventoryList.Select(i => new KeyValuePair<string, int>(i.Key, i.Value.Value)).ToList());

                    statusMessage.ForeColor = System.Drawing.Color.Green;
                    statusMessage.Text = "Your changes have been saved.";
                }
                else
                {
                    statusMessage.ForeColor = System.Drawing.Color.Red;
                    statusMessage.Text = "An error occured while saving your changes. Please try again.";
                }
            }

            if (CampaignID != null)
                CreateScreenshot((int)campaign.CampaignID, 300, 200);

            if (campaign != null && campaign.CampaignID != null)
                CampaignID = campaign.CampaignID;

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
            //CampaignSubscriptionID = campaign.SubscriptionID;

            if (campaign.SubscriptionID == null && campaign.CampaignID != null)
            {
                hdnSubscriptionTab.Value = "1";
            }
            else
            {
                hdnSubscriptionTab.Value = "0";
            }

            subscription1.CampaignID = campaign.CampaignID;
            subscription1.DataBind();

            txtCampaignName.Text = campaign.DisplayName;

            txtCampaignURL.Text = campaign.URL;

            //cbRedirect.Checked = campaign.Redirect;
            if (!string.IsNullOrEmpty(campaign.RedirectURL))
                ddlRedirectURL.SelectedValue = campaign.RedirectURL;

            cbMerchant.Checked = campaign.IsMerchant;

            cbSave.Checked = campaign.IsSave;

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
                        FillUpsellAdditional(campaignPage.CampaignPageID, txtUpsell1Code, txtUpsell1Price, txtUpsell1Quantity, CampaignSubscriptionUpsell1, cbUpsell1PlanChange);
                        //TODO: load other upsell fields
                        break;

                    case PageTypeEnum.Upsell_2:
                        txtUpsell2Title.Text = campaignPage.Title;
                        txtUpsell2HeaderHTML.Text = campaignPage.Header;
                        txtUpsell2PageHTML.Text = campaignPage.HTML;
                        FillUpsellAdditional(campaignPage.CampaignPageID, txtUpsell2Code, txtUpsell2Price, txtUpsell2Quantity, CampaignSubscriptionUpsell2, cbUpsell2PlanChange);
                        //TODO: load other upsell fields
                        break;

                    case PageTypeEnum.Upsell_3:
                        txtUpsell3Title.Text = campaignPage.Title;
                        txtUpsell3HeaderHTML.Text = campaignPage.Header;
                        txtUpsell3PageHTML.Text = campaignPage.HTML;
                        FillUpsellAdditional(campaignPage.CampaignPageID, txtUpsell3Code, txtUpsell3Price, txtUpsell3Quantity, CampaignSubscriptionUpsell3, cbUpsell3PlanChange);
                        //TODO: load other upsell fields
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

        private void FillUpsellAdditional(int campaignPageID, TextBox txtUpsellCode, TextBox txtUpsellPrice, TextBox txtUpsellQuantity, CampaignSubscriptionWithoutTrial CampaignSubscriptionUpsell, CheckBox cbUpsellPlanChange)
        {
            cbUpsellPlanChange.Checked = false;
            txtUpsellCode.Text = string.Empty;
            txtUpsellPrice.Text = string.Empty;
            txtUpsellQuantity.Text = string.Empty;
            var campaignUpsell = _cs.GetCampaignUpsell(campaignPageID);
            if (campaignUpsell != null)
            {
                if (campaignUpsell.RecurringPlanID != null)
                    cbUpsellPlanChange.Checked = true;
                txtUpsellCode.Text = campaignUpsell.ProductCode;
                txtUpsellPrice.Text = Utility.FormatPrice(campaignUpsell.Price);
                txtUpsellQuantity.Text = campaignUpsell.Quantity.ToString();
                CampaignSubscriptionUpsell.CampaignPageID = campaignPageID;
            }
        }
    }
}