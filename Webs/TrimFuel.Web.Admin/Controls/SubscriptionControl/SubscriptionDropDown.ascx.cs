using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionDropDown : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IList<RecurringPlanView> RecurringPlanList { get; set; }
        public int? SelectedRecurringPlanID { get; set; }
        public RecurringPlanView SelectedRecurringPlan 
        {
            get 
            {
                if (SelectedRecurringPlanID == null)
                    return null;
                return RecurringPlanList.Single(i => i.RecurringPlanID == SelectedRecurringPlanID);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rPlans.DataSource = RecurringPlanList;
        }
    }
}