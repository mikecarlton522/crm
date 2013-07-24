using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class OrderRecurringPlans : System.Web.UI.Page
    {
        private OrderService service = new OrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? BillingID 
        {
            get { return Utility.TryGetLong(Request["billingId"]); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (BillingID != null)
            {
                rPlans.DataSource = service.GetPlanList(BillingID.Value).Where(i => i.RecurringStatus != RecurringStatusEnum.New);
            }
        }
    }
}