using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class RecurringPlanList : System.Web.UI.Page
    {
        SubscriptionNewService service = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public int? SelectedProductID 
        {
            get { return Utility.TryGetInt(Request["productID"]); }
        }

        public string SelectedGroupProductSKU
        {
            get { return Utility.TryGetStr(Request["groupProductSKU"]); }
        }

        public int? SelectedPlanFrequency
        {
            get { return Utility.TryGetInt(Request["planFrequency"]); }
        }

        public int? SelectedRecurringPlanID
        {
            get { return Utility.TryGetInt(Request["selectedRecurringPlanID"]); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (SelectedProductID != null && !string.IsNullOrEmpty(SelectedGroupProductSKU) && SelectedPlanFrequency != null)
            {
                rpSubscriptions.DataSource = service.GetPlanList(SelectedProductID.Value, SelectedGroupProductSKU, SelectedPlanFrequency.Value);                
            }
        }
    }
}
