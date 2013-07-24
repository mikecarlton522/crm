using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Model;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Model.Containers;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductFlowWizard : System.Web.UI.Page
    {
        private CampaignService _campaignService = new CampaignService();
        private ProductService _productService = new ProductService();
        private PageService _pageService = new PageService();
        private EmailService _emailService = new EmailService();
        private LeadService _leadService = new LeadService();

        public int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productID"]);
            }
        }

        public int CampaignID
        {
            get
            {
                int campaignID;
                bool isCampaignExists = int.TryParse(Request["campaignID"], out campaignID);
                if (isCampaignExists)
                    return campaignID;
                else
                    return 0;
            }
        }

        private CampaignView campaignView = null;
        public CampaignView CampaignView
        {
            get
            {
                if (campaignView == null)
                {
                    if (Session["campaign"] == null)
                    {
                        // get Campaign
                        int campaignID;
                        bool isCampaignExists = int.TryParse(Request["campaignID"], out campaignID);
                        if (isCampaignExists)
                        {
                            campaignView = _campaignService.GetCampaignView(campaignID);
                        }
                        else
                        {
                            campaignView = new CampaignView()
                            {
                                Campaign = new Campaign()
                                {
                                    Active = true,
                                    IsSTO = false,
                                    SendUserEmail = true,
                                    IsMerchant = false,
                                    IsExternal = false,
                                    IsDupeChecking = false,
                                    IsRiskScoring = false,
                                    IsSave = false,
                                },
                                Affiliates = new List<CampaignAffiliate>(),
                                LeadPartners = new List<CampaignLeadRouting>()
                            };
                        }
                    }
                    else
                    {
                        return ((CampaignView)Session["campaign"]);
                    }
                }
                return campaignView;
            }
            set
            {
                campaignView = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            phError.Visible = false;
            phSuccess.Visible = false;


            if (!IsPostBack)
            {
                DataBind();

                // set values
                txtCampaignName.Text = CampaignView.Campaign.DisplayName;
                txtCampaignURL.Text = CampaignView.Campaign.URL;
                cbExternal.Checked = CampaignView.Campaign.IsExternal;

                if (CampaignView.Campaign.ShipperID == null)
                {
                    IList <ShipperProductView> productShippers = _productService.GetShipperProductList(ProductID.Value);
                    foreach (var item in productShippers)
                    {
                        CampaignView.Campaign.ShipperID = item.ShipperID;
                        break;
                    }
                }
                ddShipper.SelectedValue = CampaignView.Campaign.ShipperID.ToString();

                //cbRiskScoring.Checked = CampaignView.Campaign.IsRiskScoring;
                //cbDupeChecking.Checked = CampaignView.Campaign.IsDupeChecking;
            }

        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            Session["campaign"] = null;

            // populate datasources for drop-down lists
            ddShipper.DataSource = _productService.GetShippers();
        }

        protected void OnActiveStepChanged(object sender, EventArgs e)
        {
            Label lblWizardHeader = wSteps.FindControl("HeaderContainer$lblHeader") as Label;
            Image imgWizardHeader = wSteps.FindControl("HeaderContainer$imgHeader") as Image;

            if (lblWizardHeader != null)
            {
                switch (wSteps.ActiveStepIndex)
                {
                    case 0:
                        if (CampaignID > 0)
                            lblWizardHeader.Text = "Step 1: Edit Campaign";
                        else
                            lblWizardHeader.Text = "Step 1: New Campaign";
                        imgWizardHeader.ImageUrl = "images/flow-wizard-step1.png";
                        break;
                    case 1:
                        lblWizardHeader.Text = "Step 2: Select Products/Plans";
                        imgWizardHeader.ImageUrl = "images/flow-wizard-step2.png";
                        break;
                    case 2:
                        lblWizardHeader.Text = "Step 3: Select Affiliates";
                        imgWizardHeader.ImageUrl = "images/flow-wizard-step3.png";
                        break;
                    case 3:
                        lblWizardHeader.Text = "Step 4: Select Transactional Emails";
                        imgWizardHeader.ImageUrl = "images/flow-wizard-step4.png";
                        break;
                    case 4:
                        lblWizardHeader.Text = "Step 5: Lead Routing";
                        imgWizardHeader.ImageUrl = "images/flow-wizard-step5.png";
                        break;
                    case 5:
                        lblWizardHeader.Text = "Step 6: Confirmation";
                        imgWizardHeader.ImageUrl = "images/flow-wizard-step6.png";
                        break;
                }
            }

            if (wSteps.ActiveStepIndex == wSteps.WizardSteps.IndexOf(this.wStep3))
            {
                string res = "";
                foreach (var item in CampaignView.Affiliates)
                {
                    res += GetAffiliatesAsText(item.AffiliateID.Value);
                }
                ltAffiliate.Text = res;
            }

            if (wSteps.ActiveStepIndex == wSteps.WizardSteps.IndexOf(this.wStep5))
            {
                ltPartner1.Text = GetLeadTypesAsText(1);
                ltPartner2.Text = GetLeadTypesAsText(2);
                ltPartner3.Text = GetLeadTypesAsText(3);
            }

            if (wSteps.ActiveStepIndex == wSteps.WizardSteps.IndexOf(this.wStep6))
            {
                lblCampaignName.Text = CampaignView.Campaign.DisplayName;
                lblCampaignURL.Text = CampaignView.Campaign.URL;

                string productName = "";
                string subscriptionName = "";
                if (CampaignView.Campaign.SubscriptionID != null)
                {
                    Subscription subscription = _productService.GetProductSubscription(CampaignView.Campaign.SubscriptionID);
                    if (subscription != null)
                    {
                        Product product = _productService.GetProduct(subscription.ProductID);
                        productName = product.ProductName;
                        subscriptionName = GetInfo(subscription.DisplayName);
                    }
                }
                lblProduct.Text = productName;

                lblSubscription.Text = subscriptionName;

                string shipperName = "";
                if (CampaignView.Campaign.ShipperID != null)
                {
                    shipperName = ddShipper.SelectedItem.Text;
                }
                lblShipper.Text = GetInfo(shipperName);

                lblAffiliates.Text = GetInfo(GetAffiliatesAsInfo());
                lblEmails.Text = GetInfo(GetDynamicEmailsAsInfo());

                lblAbandons.Text = GetInfo(GetLeadPartnersAsInfo(1));
                lblConfirmations.Text = GetInfo(GetLeadPartnersAsInfo(2));
                lblCancellations.Text = GetInfo(GetLeadPartnersAsInfo(3));
            }
        }

        protected string GetInfo(string str)
        {
            if (str == null || str.Length == 0)
                return "Not enabled";
            else
                return str;
        }

        protected void OnNextButtonClick(Object sender, EventArgs e)
        {
            switch (wSteps.ActiveStepIndex)
            {
                case 0: // Step 1: New Campaign
                    CampaignView.Campaign.DisplayName = txtCampaignName.Text;
                    CampaignView.Campaign.IsExternal = cbExternal.Checked;
                    CampaignView.Campaign.URL = txtCampaignURL.Text;
                    break;
                case 1: // Step 2: Select Products/Plans
                    // Subscription
                    int subscriptionID = int.TryParse(Request["subscription"], out subscriptionID) ? subscriptionID : 0;
                    if (subscriptionID > 0)
                        CampaignView.Campaign.SubscriptionID = subscriptionID;

                    // Shipper
                    if (ddShipper.SelectedIndex > 0)
                        CampaignView.Campaign.ShipperID = Convert.ToInt32(ddShipper.SelectedItem.Value);
                    else
                        CampaignView.Campaign.ShipperID = null;
                    break;
                case 2: // Step 3: Select Affiliates
                    CampaignView.Campaign.IsRiskScoring = false; //cbRiskScoring.Checked;
                    CampaignView.Campaign.IsDupeChecking = false; //cbDupeChecking.Checked;

                    CampaignView.Affiliates.Clear();
                    if (Request["affiliateid"] != null)
                        ProcessAffiliatesList(Request["affiliateid"]);
                    break;
                case 3: // Step 4: Select Transactional Emails
                    break;
                case 4: // Step 5: Lead Routing
                    CampaignView.LeadPartners.Clear();
                    if (Request["partnerid1"] != null)
                        ProcessLeadPartnersList(1, Request["partnerid1"], Request["percentage1"]);
                    if (Request["partnerid2"] != null)
                        ProcessLeadPartnersList(2, Request["partnerid2"], Request["percentage2"]);
                    if (Request["partnerid3"] != null)
                        ProcessLeadPartnersList(3, Request["partnerid3"], Request["percentage3"]);
                    break;
            }
            Session["campaign"] = CampaignView;
        }

        protected void OnFinishButtonClick(Object sender, EventArgs e)
        {
            if (CampaignView.Campaign.CampaignID == null)
                CampaignView.Campaign.CreateDT = DateTime.Now;

            BusinessError<CampaignView> campaign = _campaignService.SaveCampaignView(CampaignView);

            if (campaign.State == BusinessErrorState.Success)
            {
                phSuccess.Visible = true;
                
            }
            else
            {
                phError.Visible = true;
            }
        }

        // Affiliates processing
        private IList<Affiliate> _affiliateList = null;
        protected string AffiliateList(string selected)
        {
            if (_affiliateList == null)
                _affiliateList = _pageService.GetAffiliates();

            string res = "";
            foreach (var item in _affiliateList)
            {
                string isSelected = (selected == item.AffiliateID.ToString() ? " selected" : "");
                res += "<option value='" + item.AffiliateID.ToString() + "'" + isSelected + ">" + item.Code + "</option>";
            }
            return res;
        }

        protected string GetAffiliatesAsText(int AffiliateID)
        {
            string res = "<div style=\"margin:5px;width:100%;\">" +
                "<select name=\"affiliateid\" id=\"affiliateid" + AffiliateID.ToString() + "\"" +
                "  style=\"width:200px;\" onchange=\"javascript:SelectAffiliate(this.value);\" class=\"validate[required]\">" +
                "<option value=\"\">-- Select --</option>";
            res += AffiliateList(AffiliateID.ToString());
            res += "</select>";
            res += "<a href=\"javascript:void(0)\" onclick=\"return editAffiliate(this);\" class=\"editIcon\">Edit Pixels</a>";
            res += "<a href=\"javascript:void(0)\" onclick=\"return newAffiliate(this);\" class=\"addNewIcon\">New</a>";
            res += "<a href=\"javascript:void(0)\" onclick=\"return removeAffiliate(this);\" class=\"removeIcon\">Remove</a></div>";
            return res;
        }

        protected string GetAffiliatesAsInfo()
        {
            string res = "";

            if (_affiliateList == null)
                _affiliateList = _pageService.GetAffiliates();

            foreach (var item in CampaignView.Affiliates)
            {
                foreach (var subitem in _affiliateList)
                {
                    if (item.AffiliateID == subitem.AffiliateID)
                    {
                        if (res != "")
                            res += ", ";
                        res += subitem.Code;
                        break;
                    }
                }
            }

            return res;
        }

        protected string GetDynamicEmailsAsInfo()
        {
            string res = "";
            IList<CampaignEmailTypeView> emails = _emailService.GetDynamicEmailByCampaign(ProductID.Value, CampaignID);
            foreach (CampaignEmailTypeView item in emails)
            {
                if (item.CampaignID != null && item.Active == true)
                {
                    if (res != "")
                        res += ", ";
                    res += item.DisplayName;
                }
            }

            return res;
        }

        // LeadPartners processing
        private IList<LeadPartner> _leadPartnerList = null;
        protected string LeadPartnerList(string selected)
        {
            if (_leadPartnerList == null)
                _leadPartnerList = _leadService.GetPartnerList();

            string res = "";
            foreach (var item in _leadPartnerList)
            {
                string isSelected = (selected == item.LeadPartnerID.ToString() ? " selected" : "");
                res += "<option value='" + item.LeadPartnerID.ToString() + "'" + isSelected + ">" + item.DisplayName + "</option>";
            }
            return res;
        }

        protected string GetLeadTypesAsText(int LeadTypeID)
        {
            string res = "";
            foreach (var item in CampaignView.LeadPartners)
            {
                if (item.LeadTypeID == LeadTypeID)
                    res += GetLeadPartnersAsText(LeadTypeID, item.LeadPartnerID.Value, item.Percentage == null ? 0 : item.Percentage.Value);
            }
            return res;
        }

        protected string GetLeadPartnersAsText(int leadTypeID, int leadPartnerID, int percentage)
        {
            string res = "<div style=\"margin:5px;width:100%;\">" +
                "<input type=\"text\" name=\"percentage" + leadTypeID.ToString() + "\" class=\"xnarrow\" maxlength=\"3\" value=\"" + percentage.ToString() + "\"/>%&nbsp;&nbsp;" +
                "<select name=\"partnerid" + leadTypeID.ToString() + "\" id=\"partnerid" + leadTypeID.ToString() + leadPartnerID.ToString() + "\"" +
                " style=\"width:200px;\" class=\"validate[required]\">" +
                "<option value=\"\">-- Select --</option>";
            res += LeadPartnerList(leadPartnerID.ToString());
            res += "</select>";
            res += "<a href=\"#\" onclick=\"return removePartner(this);\" class=\"removeIcon\">Remove</a></div>";
            return res;
        }

        protected string GetLeadPartnersAsInfo(int leadTypeID)
        {
            string res = "";

            if (_leadPartnerList == null)
                _leadPartnerList = _leadService.GetPartnerList();

            foreach (var item in CampaignView.LeadPartners)
            {
                if (item.LeadTypeID != leadTypeID)
                    continue;

                foreach (var subitem in _leadPartnerList)
                {
                    if (item.LeadPartnerID == subitem.LeadPartnerID)
                    {
                        if (res != "")
                            res += ", ";
                        res += subitem.DisplayName;
                        break;
                    }
                }
            }

            return res;
        }

        protected void ProcessAffiliatesList(string inputData)
        {
            string[] data = inputData.Split(new string[] { "," }, StringSplitOptions.None);
            for (int i = 1; i < data.Length; i++)
            {
                if (data[i] == "")
                    continue;

                int affiliateID = Utility.TryGetInt(data[i]).Value;
                bool foundDuplicate = false;

                foreach (var item in CampaignView.Affiliates)
                {
                    if (item.AffiliateID == affiliateID)
                        foundDuplicate = true;
                }

                if (foundDuplicate)
                    continue;

                CampaignAffiliate aff = new CampaignAffiliate();
                aff.CampaignID = CampaignView.Campaign.CampaignID;
                aff.AffiliateID = affiliateID;
                CampaignView.Affiliates.Add(aff);
            }
        }

        protected void ProcessLeadPartnersList(int leadTypeID, string inputIDs, string inputPercents)
        {
            string[] strLeadPartnerID = inputIDs.Split(new string[] { "," }, StringSplitOptions.None);
            string[] strPercentage = inputPercents.Split(new string[] { "," }, StringSplitOptions.None);
            for (int i = 1; i < strLeadPartnerID.Length; i++)
            {
                if (Utility.TryGetInt(strLeadPartnerID[i]) == null ||
                    Utility.TryGetInt(strPercentage[i]) == null ||
                    Utility.TryGetInt(strPercentage[i]) <= 0 ||
                    Utility.TryGetInt(strPercentage[i]) > 100)
                    continue;

                int leadPartnerID = Utility.TryGetInt(strLeadPartnerID[i]).Value;
                int percentage = Utility.TryGetInt(strPercentage[i]).Value;
                bool foundDuplicate = false;

                foreach (var item in CampaignView.LeadPartners)
                {
                    if (item.LeadTypeID == leadTypeID && item.LeadPartnerID == leadPartnerID)
                        foundDuplicate = true;
                }

                if (foundDuplicate)
                    continue;

                CampaignLeadRouting lcr = new CampaignLeadRouting();
                lcr.CampaignID = CampaignView.Campaign.CampaignID;
                lcr.LeadTypeID = leadTypeID;
                lcr.LeadPartnerID = leadPartnerID;
                lcr.Percentage = percentage;
                CampaignView.LeadPartners.Add(lcr);
            }
        }
    }
}
