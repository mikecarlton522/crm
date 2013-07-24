using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class OrderRecurringPlan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SubscriptionControl1.OrderRecurringPlanID = OrderRecurringPlanID;

            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? OrderRecurringPlanID 
        {
            get { return Utility.TryGetLong(Request["id"]); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);            
        }
    }
}