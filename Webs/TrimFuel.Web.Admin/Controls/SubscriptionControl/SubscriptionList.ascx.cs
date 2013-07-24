using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionList : System.Web.UI.UserControl
    {
        private SubscriptionNewService service = new SubscriptionNewService();

        public int? ProductID { get; set; }
        public string GroupProductSKU { get; set; }
        public int? PlanFrequency { get; set; }
        public int? SelectedRecurringPlanID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            //if (ProductID != null && !string.IsNullOrEmpty(GroupProductSKU) && PlanFrequency != null)
            //{
            //    rProducts.DataSource = service.GetPlanList(ProductID.Value, GroupProductSKU, PlanFrequency.Value);
            //}
            if (ProductID != null && !string.IsNullOrEmpty(GroupProductSKU) && PlanFrequency != null)
            {
                SubscriptionDropDown1.RecurringPlanList = service.GetPlanList(ProductID.Value, GroupProductSKU, PlanFrequency.Value);
                SubscriptionDropDown1.SelectedRecurringPlanID = SelectedRecurringPlanID;
            }
        }
    }
}