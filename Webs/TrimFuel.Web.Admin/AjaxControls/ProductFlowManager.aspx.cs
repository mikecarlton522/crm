using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using System.Text;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductFlowManager : System.Web.UI.Page
    {
        ProductService ps = new ProductService();

        protected int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productId"]) == null ? Utility.TryGetInt(hdnProductID.Value) : Utility.TryGetInt(Request["productId"]);
            }
        }

        protected Dictionary<string, string> Subscriptions = null;

        private List<Campaign> _campaigns = null;
        protected List<Campaign> Campaigns
        {
            get
            {
                if (_campaigns == null)
                {
                    if (lbHideExternal.Enabled)
                        _campaigns = ps.GetProductCampaigns(ProductID.Value).OrderBy(u => u.DisplayName).ToList();
                    else
                        _campaigns = ps.GetProductCampaigns(ProductID.Value).Where(u => u.IsExternal == false).OrderBy(u => u.DisplayName).ToList();
                }
                return _campaigns;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProductID == null)
                return;
            if (!IsPostBack)
            {
                EnableButtons(false, true);
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            _campaigns = null;
            var subscriptions = ps.GetProductSubscriptions(ProductID.Value).ToList();
            var currencyObj = ps.GetProductCurrency(ProductID.Value);
            string currency = currencyObj == null ? "$" : currencyObj.HtmlSymbol;
            currency = HttpUtility.HtmlDecode(currency);
            Subscriptions = new Dictionary<string, string>();
            Subscriptions.Add("", "-- Select --");
            foreach (var subsciption in subscriptions)
            {
                StringBuilder value = new StringBuilder();
                if (subsciption.Recurring == null || !subsciption.Recurring.Value)
                    value.Append("One time sale ");
                value.Append(subsciption.Quantity == null ? "1" : subsciption.Quantity.ToString());
                value.Append("x" + subsciption.ProductCode + " " + Utility.FormatCurrency(subsciption.InitialBillAmount + subsciption.InitialShipping, currency));
                if (subsciption.Recurring != null && subsciption.Recurring.Value)
                {
                    value.Append(" -> ");
                    value.Append(subsciption.QuantitySKU2 == null ? "1" : subsciption.QuantitySKU2.ToString() + "x" + subsciption.SKU2 + " " + Utility.FormatCurrency(subsciption.RegularBillAmount + subsciption.RegularShipping, currency));
                }
                Subscriptions.Add(subsciption.SubscriptionID.ToString(), value.ToString());
            }
            base.OnDataBinding(e);
            rFlow.DataSource = Campaigns;
        }

        protected void EnableButtons(bool bShow, bool bHide)
        {
            lbShowExternal.Enabled = bShow;
            lbHideExternal.Enabled = bHide;
        }

        protected void HideExternalCampaigns(object sender, EventArgs e)
        {
            EnableButtons(true, false);
            DataBind();
        }

        protected void ShowExternalCampaigns(object sender, EventArgs e)
        {
            EnableButtons(false, true);
            DataBind();
        }

        protected void rFlow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "hide")
            {
                int? cID = Convert.ToInt32(e.CommandArgument);
                string CampName = Campaigns.Where(u => u.CampaignID == cID).SingleOrDefault().DisplayName;
                Note.Text = "Campaign " + CampName + " was successfuly disabled";
                ps.HideCampaign(Convert.ToInt32(e.CommandArgument));
            }
            if (e.CommandName == "show")
            {
                int? cID = Convert.ToInt32(e.CommandArgument);
                string CampName = Campaigns.Where(u => u.CampaignID == cID).SingleOrDefault().DisplayName;
                Note.Text = "Campaign " + CampName + " was successfuly activated";
                ps.ShowCampaign(Convert.ToInt32(e.CommandArgument));
            }
            DataBind();            
        }
    }
}
