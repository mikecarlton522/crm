using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class VipPlan : System.Web.UI.Page
    {
        SubscriptionPlanService service = new SubscriptionPlanService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (SubscriptionPlanID != null)
                {
                    VIPPlan1.ViewMode = TrimFuel.Web.Admin.Controls.ViewModeEnum.View;
                }
                else
                {
                    VIPPlan1.ViewMode = TrimFuel.Web.Admin.Controls.ViewModeEnum.Edit;
                }
                DataBind();
            }
            else
            {
                SetData();
            }
        }

        public int? SubscriptionPlanID 
        {
            get 
            { 
                return Utility.TryGetInt(Request["planID"]);
            }
        }

        protected IList<SubscriptionPlanItem> AllPlanItems { get; set; }
        protected IList<SubscriptionPlanItemActionView> AllPlanItemActions { get; set; }
        protected SubscriptionPlan Plan { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            SetData();
        }

        private void SetData()
        {
            if (SubscriptionPlanID != null)
            {
                Plan = service.Load<SubscriptionPlan>(SubscriptionPlanID);                
            }
            else
            {
                Plan = null;
            }
            VIPPlan1.Plan = Plan;

            AllPlanItems = service.GetSubscriptionPlanItems();
            AllPlanItemActions = service.GetSubscriptionPlanItemActions();

            VIPPlan1.AllPlanItems = AllPlanItems;
            VIPPlan1.AllPlanItemActions = AllPlanItemActions;
        }
    }
}
