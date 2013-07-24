using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class campaigns_vipplans : PageX
    {
        SubscriptionPlanService service = new SubscriptionPlanService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected IList<SubscriptionPlanItem> AllPlanItems { get; set; }
        protected IList<SubscriptionPlanItemActionView> AllPlanItemActions { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rpPlanList.DataSource = service.GetSubscriptionPlans();
            AllPlanItems = service.GetSubscriptionPlanItems();
            AllPlanItemActions = service.GetSubscriptionPlanItemActions();

            rpPlanList2.DataSource = service.GetSubscriptionPlans();
        }

        public override string HeaderString
        {
            get { return "Loyalty Plans"; }
        }
    }
}
