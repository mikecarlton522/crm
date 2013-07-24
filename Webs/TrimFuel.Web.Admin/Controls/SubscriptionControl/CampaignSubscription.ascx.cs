using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class CampaignSubscription : System.Web.UI.UserControl
    {
        SubscriptionNewService service = new SubscriptionNewService();

        public string GenerateID
        {
            get;
            private set;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GenerateID = "SubscriptionControl_" + Utility.RandomString(new Random(), 5);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                SelectedInventoryList = new List<KeyValuePair<string, int?>>();
                //Response.Write(Request["inventory"] + "<br/>");
                //Response.Write(Request["quantity"] + "<br/>");
                if (Request["inventory"] != null)
                {
                    string[] inventory = Request["inventory"].Split(new string[] { "," }, StringSplitOptions.None);
                    string[] quantity = Request["quantity"].Split(new string[] { "," }, StringSplitOptions.None);
                    for (int i = 1; i < inventory.Length; i++)
                    {
                        SelectedInventoryList.Add(new KeyValuePair<string, int?>(inventory[i], Utility.TryGetInt(quantity[i])));
                    }
                }
            }
            else
            {
                DataBind();
            }
        }

        public int? CampaignID { get; set; }

        public int? SelectedRecurringPlanID
        {
            get
            {
                return Subscription1.SelectedRecurringPlanID;
            }
        }

        public decimal? SelectedTrialPrice
        {
            get
            {
                return Utility.TryGetDecimal(tbPrice.Text);
            }
        }
        public int? SelectedTrialPeriod
        {
            get
            {
                return Utility.TryGetInt(tbTrialInterim.Text);
            }
        }

        public IList<KeyValuePair<string, int?>> SelectedInventoryList { get; set; }

        private IList<ProductSKU> inventoryList = null;
        protected string InventoryOptionList(string selected)
        {
            if (inventoryList == null)
            {
                inventoryList = service.GetProductList();
            }

            string res = "";
            foreach (var item in inventoryList)
            {
                string isSelected = (selected == item.ProductSKU_ ? " selected" : "");
                res += "<option value='" + item.ProductSKU_ + "'" + isSelected + ">" + item.ProductSKU_ + "</option>";
            }
            return res;
        }

        private bool isBound = false;
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            isBound = true;

            CampaignRecurringPlanView campaignPlan = null;
            if (CampaignID != null)
            {
                campaignPlan = service.GetRecurringPlanByCampaign(CampaignID.Value);
            }
            else
            {
                campaignPlan = new CampaignRecurringPlanView();
                campaignPlan.ProductList = new List<CampaignTrialProduct>();
            }

            if (SelectedInventoryList == null)
            {
                SelectedInventoryList = campaignPlan.ProductList.Select(i => new KeyValuePair<string, int?>(i.ProductSKU, i.Quantity)).ToList();
            }

            Subscription1.RecurringPlan = campaignPlan.RecurringPlan;
            if (campaignPlan.CampaignRecurringPlan != null)
            {
                tbPrice.Text = campaignPlan.CampaignRecurringPlan.TrialPrice.ToString();
                tbTrialInterim.Text = campaignPlan.CampaignRecurringPlan.TrialInterim.ToString();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!isBound)
            {
                if (Subscription1.SelectedRecurringPlanID != null)
                {
                    Subscription1.RecurringPlan = service.GetPlan(Subscription1.SelectedRecurringPlanID.Value);
                }
                Subscription1.DataBind();
                phProducts.DataBind();
            }
        }

        public string Validate()
        {
            string res = null;
            if (SelectedRecurringPlanID == null)
            {
            }
            else
            {
                if (SelectedTrialPeriod == null)
                {
                    res = "Please specify Trial Interim";
                }
                else if (SelectedTrialPrice == null)
                {
                    res = "Please specify Trial Price";
                }
                else if (SelectedInventoryList.Count > 0)
                {
                    foreach (var item in SelectedInventoryList)
                    {
                        if (string.IsNullOrEmpty(item.Key))
                        {
                            res = "Please specify valid products in Trial Product section";
                            break;
                        }
                        else if (item.Value == null)
                        {
                            res = "Please specify valid quantities in Trial Product section";
                            break;
                        }
                    }
                }
            }
            return res;
        }
    }
}