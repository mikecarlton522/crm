using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class BillingSignToPlan : System.Web.UI.Page
    {
        private SubscriptionPlanService service = new SubscriptionPlanService();
        private SubscriptionService sService = new SubscriptionService();
        private OrderService oService = new OrderService();

        public long? BillingID { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BillingID = Utility.TryGetLong(Request["BillingID"]);
            if (BillingID == null)
            {
                Visible = false;
            }
            else
            {
                var bsList = sService.GetBillingSubscriptionsByBilling(BillingID);
                if (bsList.Where(u => u.StatusTID == TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Active).Count() == 0)
                {
                    var planList = oService.GetPlanList(BillingID.Value);
                    if (planList.Where(u => u.RecurringStatus == TrimFuel.Model.Enums.RecurringStatusEnum.Active).Count() == 0)
                        Visible = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack && Visible)
            {
                DataBind();
            }
            phError.Visible = false;
            phSuccess.Visible = false;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            List<SubscriptionPlan> ds = new List<SubscriptionPlan>();
            ds.Add(new SubscriptionPlan()
            {
                SubscriptionPlanName = "-- Select --"
            });
            ds.AddRange(service.GetSubscriptionPlans());
            ddlSubscriptionPlanList.DataSource = ds;
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            bool res = false;
            var planList = oService.GetPlanList(BillingID.Value);
            if (planList.Where(u => u.RecurringStatus == TrimFuel.Model.Enums.RecurringStatusEnum.Active).Count() > 0)
            {
                //new structure
                res = service.SignupUserNew(BillingID.Value, Utility.TryGetInt(ddlSubscriptionPlanList.SelectedValue).Value, Logic.AdminMembership.CurrentAdmin.DisplayName, Logic.AdminMembership.CurrentCampaignID).State == BusinessErrorState.Success;
            }
            else
            {
                //old structure
                res = service.SignupUser(BillingID.Value, Utility.TryGetInt(ddlSubscriptionPlanList.SelectedValue).Value).State == BusinessErrorState.Success;
            }
            phSuccess.Visible = res;
            phError.Visible = !phSuccess.Visible;
        }
    }
}
