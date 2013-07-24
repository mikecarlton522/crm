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
    public partial class CampaignSubscriptionWithoutTrial : System.Web.UI.UserControl
    {
        static Random cRnd = new Random();
        SubscriptionNewService service = new SubscriptionNewService();
        CampaignService cService = new CampaignService();

        public string GenerateID
        {
            get;
            private set;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GenerateID = "SubscriptionControl_" + Utility.RandomString(cRnd, 5);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public int? CampaignPageID { get; set; }

        public int? SelectedRecurringPlanID
        {
            get
            {
                return Subscription1.SelectedRecurringPlanID;
            }
        }

        private bool isBound = false;
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            isBound = true;

            CampaignUpsell campaignUpsell = null;
            if (CampaignPageID != null)
            {
                campaignUpsell = cService.GetCampaignUpsell(CampaignPageID.Value);
            }
            else
            {
                campaignUpsell = new CampaignUpsell();
            }

            var plan = cService.Load<RecurringPlan>(campaignUpsell.RecurringPlanID);
            if (plan != null)
                Subscription1.RecurringPlan = service.GetPlan(plan.RecurringPlanID.Value);
            else
                Subscription1.RecurringPlan = null;
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
            }
        }

        public string Validate()
        {
            //string res = null;
            return null;
        }
    }
}